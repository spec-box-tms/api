using System.Security.Cryptography;
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

    /// <summary>
    /// Returns the list of projects.
    /// </summary>
    [HttpGet(Name = "ListProjects")]
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
            Versions = g.Select(p => new VersionModel
            {
                Version = p.Version,
                UpdatedAt = p.UpdatedAt
            }).ToArray()
        }).ToArray();

        return Json(projectsGrouped);
    }

    /// <summary>
    /// Returns the feature details.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="feature">The feature code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An 200 SUCCESS representing the HTTP response.</returns>
    [HttpGet("{project}/features/{feature}", Name = "GetFeature")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureModel>> Feature(string project, string feature, [FromQuery(Name = "version")] string? version)
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

    /// <summary>
    /// Returns the list of features for a specific project.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An array of FeatureModel objects representing the retrieved project features.</returns>
    [HttpGet("{project}/structures:plain", Name = "GetStructurePlain")]
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

    /// <summary>
    /// Returns the list of structures for a specific project.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An array of TreeModel objects representing the retrieved project structures.</returns>
    [HttpGet("{project}/structures", Name = "ListStructures")]
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

    /// <summary>
    /// Returns the structure details.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="treeCode">The tree code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>The tree structure of retrived project tree.</returns>
    [HttpGet("{project}/structures/{treeCode}", Name = "GetStructure")]
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

    /// <summary>
    /// Returns the graph of feature and attribute rlations.
    /// </summary>
    /// <param name="project">The project code. </param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns> Graph of feature and attribute relations</returns>
    [HttpGet("{project}/features:relations", Name = "GetFeatureRelations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureRelationsModel>> GetFeatureRelations(string project, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var featureRelations = await (from featureTarget in db.Features.Where(f => f.ProjectId == prj.Id)
                                      from featureSource in db.Features.Where(f => f.ProjectId == prj.Id &&
                                          (
                                            f.Title.Contains("$" + featureTarget.Code) ||
                                            (f.Description != null && f.Description.Contains("$" + featureTarget.Code)) ||

                                                f.AssertionGroups.Any(g => g.Title.Contains("$" + featureTarget.Code)) ||
                                                
                                                f.AssertionGroups.Any(g =>
                                                    g.Assertions.Any(a => a.Title.Contains("$" + featureTarget.Code) ||
                                                    (a.Description != null && a.Description.Contains("$" + featureTarget.Code)))
                                                )
                                        )
                                      )
                                      select new GraphEdgeModel
                                      {
                                          Source = "feature:" + featureSource.Code,
                                          Target = "feature:" + featureTarget.Code,
                                      }).Distinct().ToArrayAsync();

        var attributeValueRelations = await (from attributeValueTarget in db.AttributeValues.Where(a => a.Attribute.ProjectId == prj.Id)
                                             from featureSource in db.Features.Where(f => f.ProjectId == prj.Id &&
                                                 attributeValueTarget.Features.Any(af => af.Id == f.Id))
                                             select new GraphEdgeModel
                                             {
                                                 Source = "feature:" + featureSource.Code,
                                                 Target = "attribute-value:" + attributeValueTarget.Code,
                                             }).Distinct().ToArrayAsync();

        var featureNodes = await db.Features.Where(f => f.ProjectId == prj.Id).Select(f => new GraphNodeModel
        {
            Id = "feature:" + f.Code,
            Title = f.Title
        }).ToArrayAsync();

        var attributeValueNodes = await db.AttributeValues.Where(a => a.Attribute.ProjectId == prj.Id).Select(a => new GraphNodeModel
        {
            Id = "attribute-value:" + a.Code,
            Title = a.Title != null ? a.Title : a.Code
        }).ToArrayAsync();

        var relations = featureRelations.Concat(attributeValueRelations).ToArray();
        var nodes = featureNodes.Concat(attributeValueNodes).ToArray();

        var result = new FeatureRelationsModel
        {
            Nodes = nodes,
            Edges = relations
        };

        return Json(result);
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
