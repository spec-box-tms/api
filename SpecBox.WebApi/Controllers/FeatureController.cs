
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.WebApi.Model.Project;
using SpecBox.WebApi.Model.Project.Feature;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("projects/{projectCode}/{version}/features")]
public class FeatureController(ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Детали фичи
    /// </summary>
    [HttpGet(Name = "ListFeatures")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureResponse[]>> List(string projectCode, string version)
    {
        var f = await db.Features
            .Include(f => f.Attributes)
            .ThenInclude(a => a.Attribute)
            .Where(f => f.Project.Code == projectCode && f.Project.Version == version)
            .OrderBy(f => f.Title)
            .ToListAsync();
        if (f == null) return NotFound();

        var features = mapper.Map<FeatureResponse[]>(f);

        return Json(features);
    }

    /// <summary>
    /// Детали фичи
    /// </summary>
    [HttpGet("{featureCode}", Name = "GetFeature")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureModel>> Feature(string projectCode, string version, string featureCode)
    {
        try
        {
            var f = await db.Features
                .Include(f => f.AssertionGroups)
                .ThenInclude(g => g.Assertions)
                .Include(f => f.Attributes)
                .ThenInclude(a => a.Attribute)
                .SingleOrDefaultAsync(f => f.Code == featureCode && f.Project.Code == projectCode && f.Project.Version == version);
            if (f == null) return NotFound();

            var model = mapper.Map<FeatureModel>(f);

            return Json(model);
        }
        catch (InvalidOperationException)
        {
            return Problem("Feature duplicate found", "Feature", StatusCodes.Status500InternalServerError);
        }
    }

}