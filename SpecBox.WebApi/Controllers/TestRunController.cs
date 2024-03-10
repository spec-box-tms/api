using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Model;
using SpecBox.WebApi.Model.Common;
using SpecBox.WebApi.Model.TestRun;

namespace SpecBox.WebApi.Controllers;

[ApiController]
[Route("tests")]
public class TestRunController : Controller
{
    private readonly SpecBoxDbContext db;
    private readonly ILogger logger;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunController"/> class.
    /// </summary>
    /// <param name="db">The instance used for accessing the database.</param>
    /// <param name="logger">The instance used for logging.</param>
    /// <param name="mapper">The instance used for mapping.</param>
    public TestRunController(SpecBoxDbContext db, ILogger<TestRunController> logger, IMapper mapper)
    {
        this.db = db;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new test run for the specified project.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <param name="data">The object containing the test run data.</param>
    /// <returns>An 200 SUCCESS representing the HTTP response.</returns>
    [HttpPost("projects/{project}/testruns", Name = "CreateTestRun")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTestRun(string project, [FromQuery(Name = "version")] string? version, [FromBody] CreateTestRunModel data)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var now = DateTime.Now;

        var testRun = new TestRun
        {
            ProjectId = prj.Id,
            Title = data.Title,
            Description = data.Description,
            CreatedAt = now,
            Environment = data.Environment,
            Configuration = data.Configuration
        };

        await using var tran = await db.Database.BeginTransactionAsync();

        var assertions = await db.Assertions.Where(assertion => assertion.AssertionGroup.Feature.ProjectId == prj.Id).ToListAsync();

        testRun.TotalCount = assertions.Count;
        testRun.SkippedCount = assertions.Count(a => a.IsAutomated);
        db.TestRuns.Add(testRun);

        foreach (var assertion in assertions)
        {
            var testResult = new TestResult
            {
                Assertion = assertion,
                TestRun = testRun,
                Status = assertion.IsAutomated ? "SKIPPED" : "NEW",
                UpdatedAt = now
            };
            db.TestResults.Add(testResult);
        }
        await db.SaveChangesAsync();
        await tran.CommitAsync();

        return Ok();
    }

    /// <summary>
    /// Retrieves a list of test runs for a specific project from the database.
    /// </summary>
    /// <param name="project">The project code for which to retrieve the test runs.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An array of TestRunModel objects representing the retrieved test runs.</returns>
    [HttpGet("projects/{project}/testruns", Name = "ListTestRuns")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectTestRunsModel>> ListTestRuns(string project, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var testRuns = await db.TestRuns.Where(tr => tr.Project.Code == prj.Code).Select(tr => new TestRunModel
        {
            Id = tr.Id,
            Title = tr.Title,
            ProjectCode = prj.Code,
            Version = tr.Project.Version,
            CreatedAt = tr.CreatedAt,
            Description = tr.Description,
            StartedAt = tr.StartedAt,
            CompletedAt = tr.CompletedAt,
            TotalCount = tr.TotalCount,
            PassedCount = tr.PassedCount,
            FailedCount = tr.FailedCount,
            BlockedCount = tr.BlockedCount,
            InvalidCount = tr.InvalidCount,
            SkippedCount = tr.SkippedCount,
            Environment = tr.Environment,
            Configuration = tr.Configuration
        }).ToArrayAsync();

        var projectModel = mapper.Map<Project, ProjectVersionModel>(prj);
        var configurations = await _getTestRunConfigurations(prj);

        var model = new ProjectTestRunsModel
        {
            Project = projectModel,
            TestRuns = testRuns,
            Configurations = configurations
        };

        return Json(model);
    }

    /// <summary>
    /// Retrieves a specific test run by ID.
    /// </summary>
    /// <param name="testRunId">Test run ID</param>
    /// <returns>Detail model of test run and project</returns>
    [HttpGet("testruns/{testRunId}", Name = "GetTestRun")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestRunDetailsModel>> GetTestRun(Guid testRunId)
    {
        var testRun = await db.TestRuns.FirstOrDefaultAsync(tr => tr.Id == testRunId);
        if (testRun == null) return NotFound();

        var prj = await db.Projects.SingleAsync(p => p.Id == testRun.ProjectId);

        var projectModel = mapper.Map<Project, ProjectVersionModel>(prj);

        var testRunModel = new TestRunModel
        {
            Id = testRun.Id,
            Title = testRun.Title,
            ProjectCode = testRun.Project.Code,
            Version = testRun.Project.Version,
            CreatedAt = testRun.CreatedAt,
            Description = testRun.Description,
            StartedAt = testRun.StartedAt,
            CompletedAt = testRun.CompletedAt,
            TotalCount = testRun.TotalCount,
            PassedCount = testRun.PassedCount,
            FailedCount = testRun.FailedCount,
            BlockedCount = testRun.BlockedCount,
            InvalidCount = testRun.InvalidCount,
            SkippedCount = testRun.SkippedCount,
            Environment = testRun.Environment,
            Configuration = testRun.Configuration
        };

        var result = new TestRunDetailsModel
        {
            Project = projectModel,
            TestRun = testRunModel
        };

        return Json(result);
    }

    /// <summary>
    /// Retrieves test results for a specific test run.
    /// </summary>
    /// <param name="testRunId">The ID of the test run.</param>
    /// <returns>A TestResultModel representing the asynchronous operation.</returns>
    [HttpGet("testruns/{testRunId}/testresults", Name = "ListTestResults")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestResultModel[]>> ListTestResults(Guid testRunId)
    {
        var testRun = await db.TestRuns.FirstOrDefaultAsync(t => t.Id == testRunId);
        if (testRun == null) return NotFound();

        var testResults = await db.TestResults.Where(testResult => testResult.TestRunId == testRun.Id).Select(_mapTestResult).ToArrayAsync();

        return Json(testResults);
    }
    /// <summary>
    /// Retrieves a specific test result for a given project, test run, and test result ID from the database.
    /// </summary>
    /// <param name="testRunId">The ID of the test run.</param>
    /// <param name="testResultId">The ID of the test result.</param>
    /// <returns>The retrieved test result. If the project, test run, or test result is not found, it returns a 404 Not Found response.</returns>
    [HttpGet("testruns/{testRunId}/testresults/{testResultId}", Name = "GetTestResult")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestResultModel>> GetTestResult(Guid testRunId, Guid testResultId)
    {
        var testResult = await db.TestResults
            .Where(testResult => testResult.TestRunId == testRunId && testResult.Id == testResultId)
            .Select(_mapTestResult).FirstOrDefaultAsync();
        if (testResult == null) return NotFound();

        return Json(testResult);
    }
    /// <summary>
    /// Retrieves test result history.
    /// </summary>
    /// <param name="testRunId">The ID of the test run.</param>
    /// <param name="testResultId">The ID of the test result.</param>
    /// <returns>Returns history items for the same assertion in test runs and versions of the Proejct.</returns>
    [HttpGet("testruns/{testRunId}/testresults/{testResultId}/history", Name = "GetTestResultHistory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestResultHistoryModel[]>> GetTestResultHistory(Guid testRunId, Guid testResultId)
    {
        var filter = await db.TestResults
            .Where(testResult => testResult.TestRunId == testRunId && testResult.Id == testResultId)
            .Select(testResult => new
            {
                projectCode = testResult.Assertion.AssertionGroup.Feature.Project.Code,
                featureCode = testResult.Assertion.AssertionGroup.Feature.Code,
                assertionGroup = testResult.Assertion.AssertionGroup.Title,
                assertionTitle = testResult.Assertion.Title,
            }).FirstOrDefaultAsync();
        if (filter == null) return NotFound();

        var historyItems = await db.TestResults
            .Where(testResult =>
                testResult.Assertion.AssertionGroup.Feature.Project.Code == filter.projectCode &&
                testResult.Assertion.AssertionGroup.Feature.Code == filter.featureCode &&
                testResult.Assertion.AssertionGroup.Title == filter.assertionGroup &&
                testResult.Assertion.Title == filter.assertionTitle &&
                testResult.CompletedAt != null
            )
            .Select((testResult) => new TestResultHistoryModel
            {
                Id = testResult.Id,
                TestRunId = testResult.TestRunId,
                TestRunTitle = testResult.TestRun.Title,
                Version = testResult.TestRun.Project.Version,
                Status = testResult.Status,
                Report = testResult.Report,
                CompletedAt = testResult.CompletedAt!.Value
            })
            .ToListAsync();

        return Json(historyItems);
    }

    /// <summary>
    /// Updates test result with provided Status and Report.
    /// Supported Statuses: PASSED, SKIPPED, BLOCKED, INVALID, FAILED, NEW
    /// </summary>
    /// <param name="testRunId">The ID of the test run.</param>
    /// <param name="testResultId">The ID of the test result.</param>
    /// <param name="data">The data to update the test result with.</param>
    /// <returns>An updated TestResultModel.</returns>
    [HttpPut("testruns/{testRunId}/testresults/{testResultId}", Name = "UpdateTestResult")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TestResultModel>> UpdateTestResult(
        Guid testRunId,
        Guid testResultId,
        [FromBody] UpdateTestResultModel data
    )
    {
        var testRun = await db.TestRuns.FirstOrDefaultAsync(t => t.Id == testRunId);
        if (testRun == null) return NotFound();

        var testResultToUpdate = await db.TestResults.FirstOrDefaultAsync(t => t.TestRunId == testRun.Id && t.Id == testResultId);
        if (testResultToUpdate == null) return NotFound();

        switch (data.Status)
        {
            case "PASSED":
            case "SKIPPED":
                break;

            case "BLOCKED":
            case "INVALID":
            case "FAILED":
                // temporary supressed
                // if (string.IsNullOrEmpty(data.Report)) return BadRequest("Report should be set for FAIL status");
                break;
            default:
                return BadRequest($"Status {data.Status} is not supported");
        }

        if (testRun.StartedAt == null)
        {
            testRun.StartedAt = DateTime.Now;
        }

        var latestCompletedAt = await db.TestResults
            .Where(t => t.TestRunId == testRun.Id && t.CompletedAt != null)
            .OrderByDescending(t => t.CompletedAt)
            .Select(t => t.CompletedAt)
            .FirstOrDefaultAsync();

        if (testResultToUpdate.CompletedAt == null)
        {
            testResultToUpdate.StartedAt = latestCompletedAt ?? DateTime.Now;
            testResultToUpdate.CompletedAt = DateTime.Now;
        }
        testResultToUpdate.UpdatedAt = DateTime.Now;
        testResultToUpdate.Status = data.Status;
        testResultToUpdate.Report = data.Report;

        await db.SaveChangesAsync();
        await _updateTestRunStats(testRun);
        await db.SaveChangesAsync();

        var testResult = await db.TestResults
            .Where(testResult => testResult.TestRunId == testRun.Id && testResult.Id == testResultId)
            .Select(_mapTestResult).FirstOrDefaultAsync();
        if (testResult == null) return NotFound();

        return Json(testResult);
    }

    private async Task<TestRunConfigurationsModel> _getTestRunConfigurations(Project project) {
        var configurations = await db.TestRuns
            .Where(tr => tr.Project.Code == project.Code && tr.Configuration != null)
            .Select(tr => tr.Configuration!)
            .Distinct()
            .ToArrayAsync();

        var environments = await db.TestRuns
            .Where(tr => tr.Project.Code == project.Code && tr.Environment != null)
            .Select(tr => tr.Environment!)
            .Distinct()
            .ToArrayAsync();

        var result = new TestRunConfigurationsModel
        {
            Configurations = configurations,
            Environments = environments
        };
        return result;
    }

    private async Task _updateTestRunStats(TestRun testRun) {
        testRun.TotalCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id);
        testRun.PassedCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id && tr.Status == "PASSED");
        testRun.FailedCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id && tr.Status == "FAILED");
        testRun.BlockedCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id && tr.Status == "BLOCKED");
        testRun.InvalidCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id && tr.Status == "INVALID");
        testRun.SkippedCount = await db.TestResults.CountAsync(tr => tr.TestRunId == testRun.Id && tr.Status == "SKIPPED");
    }

    private Expression<Func<TestResult, TestResultModel>> _mapTestResult = (TestResult testResult) => new TestResultModel
    {
        Id = testResult.Id,
        TestRunId = testResult.TestRunId,
        Status = testResult.Status,
        Report = testResult.Report,
        UpdatedAt = testResult.UpdatedAt,
        StartedAt = testResult.StartedAt,
        CompletedAt = testResult.CompletedAt,
        AssertionTitle = testResult.Assertion.Title,
        AssertionGroupTitle = testResult.Assertion.AssertionGroup.Title,
        FeatureCode = testResult.Assertion.AssertionGroup.Feature.Code,
        FeatureTitle = testResult.Assertion.AssertionGroup.Feature.Title
    };
}
