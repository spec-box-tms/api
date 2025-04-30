using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model;
using SpecBox.WebApi.Model.Common;
using SpecBox.WebApi.Model.Project;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("projects")]
public class ProjectController(ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Returns the list of projects.
    /// </summary>
    [HttpGet(Name = "ListProjects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectResponse[]>> Projects()
    {
        var projects = await db.Projects.ToArrayAsync();

        var projectsGrouped = projects.GroupBy(p => p.Code).Select(g => new ProjectResponse
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
    /// Получить детали проекта и версий
    /// </summary>
    [HttpGet("{code}", Name = "GetProject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetProject(string code)
    {
        var projectVersions = await db.Projects.Where(p => p.Code == code).ToArrayAsync();

        if(projectVersions.Length == 0) {
            return NotFound();
        }

        var projectsGrouped = projectVersions.GroupBy(p => p.Code).Select(g => new ProjectResponse
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

        return Json(projectsGrouped[0]);
    }

    /// <summary>
    /// Returns the list of features for a specific project.
    /// </summary>
    /// <param name="project">The project code.</param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns>An array of FeatureModel objects representing the retrieved project features.</returns>
    [Obsolete("Этот метод будет удален, вместо него использовать GET:projects/{projectCode}/features")]
    [HttpGet("{project}/versions/{version}/structures:plain", Name = "GetStructurePlain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StructureModel>> StructurePlain(string project, string version)
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
    /// Returns the graph of feature and attribute relations.
    /// </summary>
    /// <param name="project">The project code. </param>
    /// <param name="version">The project version. Default version if not provided.</param>
    /// <returns> Graph of feature and attribute relations</returns>
    [HttpGet("{project}/versions/{version}/features:relations", Name = "GetFeatureRelations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureRelationsModel>> GetFeatureRelations(string project, string version)
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

    private async Task<TreeNodeModel[]> GetDefaultTreeModel(string projectCode, string version)
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
}
