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

    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectModel[]>> Projects()
    {
        var projects = await db.Projects.ToArrayAsync();

        var model = projects.Select(mapper.Map<Project, ProjectModel>).ToArray();

        return Json(model);
    }

    [HttpGet("{project}/features/{feature}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureModel>> Feature(string project, string feature)
    {
        try
        {
            var f = await db.Features
                .Include(f => f.AssertionGroups)
                .ThenInclude(g => g.Assertions)
                .SingleOrDefaultAsync(f => f.Code == feature && f.Project.Code == project);
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
    public async Task<ActionResult<StructureModel>> StructurePlain(string project)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project);
        if (prj == null) return NotFound();

        var projectModel = mapper.Map<Project, ProjectModel>(prj);

        var nodes = await GetDefaultTreeModel(project);

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
    public async Task<ActionResult<TreeModel[]>> ListStructures(string project)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project);
        if (prj == null) return NotFound();

        var trees = await db.Trees.Where(t => t.ProjectId == prj.Id).Select((tree) => new TreeModel
        {
            Code = tree.Code,
            Title = tree.Title
        }).ToArrayAsync();
        if(trees.Length == 0) return NotFound();

        return Json(trees); 
    }

    [HttpGet("{project}/structures/{treeCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StructureModel>> Structure(string project, string treeCode)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project);
        if (prj == null) return NotFound();

        var tree = await db.Trees.FirstOrDefaultAsync(t => t.ProjectId == prj.Id && t.Code == treeCode);
        if (tree == null) return NotFound();

        var projectModel = mapper.Map<Project, ProjectModel>(prj);

        var nodes = await GetTreeModel(tree);

        var model = new StructureModel
        {
            Project = projectModel,
            Tree = nodes
        };

        return Json(model);
    }

    private async Task<TreeNodeModel[]> GetDefaultTreeModel(string projectCode)
    {
        var nodes = await db.Features
            .Where(f => f.Project.Code == projectCode)
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
