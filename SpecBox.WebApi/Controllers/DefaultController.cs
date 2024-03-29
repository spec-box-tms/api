using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpecBox.WebApi.Model.Default;

namespace SpecBox.WebApi.Controllers;

public class DefaultController : Controller
{
    private IConfiguration configuration;
    private JsonOptions jsonOptions;

    public DefaultController(IConfiguration configuration, IOptions<JsonOptions> jsonOptions)
    {
        this.configuration = configuration;
        this.jsonOptions = jsonOptions.Value;
    }

    /// <summary>
    /// Проверка работоспособности приложения
    /// </summary>
    [HttpGet("ping"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Ping()
    {
        // ручка для проверки работоспособности приложения
        return Ok();
    }

    /// <summary>
    /// Получение конфигурации приложения
    /// </summary>
    [HttpGet("config")]
    [ProducesResponseType(typeof(ConfigurationModel), StatusCodes.Status200OK)]
    public IActionResult Config()
    {
        return Json(GetConfigModel());
    }

    [HttpGet("config.js"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult ConfigScript()
    {
        var configModel = GetConfigModel();
        var json = JsonSerializer.Serialize(configModel, jsonOptions.JsonSerializerOptions);

        return Content($"window.__SPEC_BOX_CONFIG={json}", "application/javascript");
    }

    private ConfigurationModel GetConfigModel()
    {
        string? counterId = configuration.GetValue<string>("MetrikaCounterId");
        ConfigurationModel model = new() { MetrikaCounterId = counterId };

        return model;
    }
}
