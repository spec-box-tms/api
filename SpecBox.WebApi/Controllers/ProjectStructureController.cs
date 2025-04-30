using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model;
using SpecBox.WebApi.Model.Common;
using SpecBox.WebApi.Model.Project;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("projects/{projectCode}/structures")]
public class ProjectStructureController(ApplicationDbContext db, IMapper mapper) : Controller
{

    /// <summary>
    /// Returns the list of structures for a specific project.
    /// </summary>
    /// <param name="projectCode">The project code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An array of TreeModel objects representing the retrieved project structures.</returns>
    [HttpGet(Name = "ListStructures")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TreeModel[]>> ListStructures(string projectCode, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == projectCode && p.Version == version);
        if (prj == null) return NotFound();

        var trees = await db.Trees.Where(t => t.ProjectId == prj.Id).Select((tree) => new TreeModel
        {
            Code = tree.Code,
            Title = tree.Title
        }).ToArrayAsync();

        if (trees.Length == 0) Json(new TreeModel[0]);

        return Json(trees);
    }

    /// <summary>
    /// Returns the structure details.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="treeCode">The tree code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>The tree structure of retrieved project tree.</returns>
    [HttpGet("{treeCode}", Name = "GetStructure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StructureModel>> Structure(string project, string treeCode, [FromQuery(Name = "version")] string? version)
    {
        var prj = await db.Projects.FirstOrDefaultAsync(p => p.Code == project && p.Version == version);
        if (prj == null) return NotFound();

        var tree = await db.Trees.FirstOrDefaultAsync(t => t.ProjectId == prj.Id && t.Code == treeCode);
        if (tree == null) return NotFound();

        var projectModel = mapper.Map<ProjectVersionModel>(prj);

        var nodes = await GetTreeModel(tree);

        var model = new StructureModel
        {
            Project = projectModel,
            Tree = nodes
        };

        return Json(model);
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