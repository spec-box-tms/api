using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Model;
using SpecBox.WebApi.Model.Common;
using SpecBox.WebApi.Model.Project;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("projects")]
public class ProjectController : Controller
{
    private readonly SpecBoxDbContext db;
    private readonly ILogger logger;
    private readonly IMapper mapper;

    public ProjectController(SpecBoxDbContext db, ILogger<ProjectController> logger, IMapper mapper)
    {
        this.db = db;
        this.logger = logger;
        this.mapper = mapper;
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectModel[]>> Projects()
    {
        var projects = await db.Projects.ToArrayAsync();

        var projectsGrouped = projects.GroupBy(p => p.Code).Select(g => new ProjectModel
        {
            Code = g.Key,
            Title = g.First().Title,
            Description = g.First().Description,
            RepositoryUrl = g.First().RepositoryUrl,
            Versions = g.Select(p => p.Version).ToArray()
        }).ToArray();

        return Json(projectsGrouped);
    }

    [HttpGet("{project}/features/{feature}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureModel>> Feature(string project,  string feature, [FromQuery(Name = "version")] string? version)
    {
        try
        {
            var f = await db.Features
                .Include(f => f.AssertionGroups)
                .ThenInclude(g => g.Assertions)
                .SingleOrDefaultAsync(f => f.Code == feature && f.Project.Code == project && f.Project.Version == version);
            if (f == null) return NotFound();

            var model = mapper.Map<FeatureModel>(f);

            return Json(model);
        }
        catch (InvalidOperationException)
        {
            return Problem("Feature duplicate found", "Feature", StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{project}/structures:plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StructureModel>> StructurePlain(string project, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var projectModel = mapper.Map<Project, ProjectVersionModel>(prj);

        var nodes = await GetDefaultTreeModel(project, version);

        var model = new StructureModel
        {
            Project = projectModel,
            Tree = nodes
        };

        return Json(model);
    }

    [HttpGet("{project}/structures")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TreeModel[]>> ListStructures(string project, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var trees = await db.Trees.Where(t => t.ProjectId == prj.Id).Select((tree) => new TreeModel
        {
            Code = tree.Code,
            Title = tree.Title
        }).ToArrayAsync();
        if (trees.Length == 0) return NotFound();

        return Json(trees);
    }

    [HttpGet("{project}/structures/{treeCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StructureModel>> Structure(string project, string treeCode, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var tree = await db.Trees.FirstOrDefaultAsync(t => t.ProjectId == prj.Id && t.Code == treeCode);
        if (tree == null) return NotFound();

        var projectModel = mapper.Map<Project, ProjectVersionModel>(prj);

        var nodes = await GetTreeModel(tree);

        var model = new StructureModel
        {
            Project = projectModel,
            Tree = nodes
        };

        return Json(model);
    }

    private async Task<TreeNodeModel[]> GetDefaultTreeModel(string projectCode, string? version)
    {
        var nodes = await db.Features
            .Where(f => f.Project.Code == projectCode && f.Project.Version == version)
            .Select(f => new TreeNodeModel
            {
                Id = f.Id,
                Title = f.Title,
                TotalCount = f.AssertionGroups.SelectMany(gr => gr.Assertions).Count(),
                AutomatedCount = f.AssertionGroups.SelectMany(gr => gr.Assertions).Count(a => a.IsAutomated),
                FeatureCode = f.Code,
            })
            .ToArrayAsync();

        return nodes;
    }

    private async Task<TreeNodeModel[]> GetTreeModel(Tree tree)
    {
        var nodes = await db.TreeNodes
            .Where(n => n.TreeId == tree.Id)
            .Select(n => new TreeNodeModel
            {
                Id = n.Id,
                ParentId = n.ParentId,
                Title = n.Title,
                TotalCount = n.Amount,
                AutomatedCount = n.AmountAutomated,
                FeatureCode = n.Feature == null ? null : n.Feature.Code,
                SortOrder = n.SortOrder,
            })
            .ToArrayAsync();

        return nodes;
    }
}
