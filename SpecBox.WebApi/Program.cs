using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using SpecBox.Domain;
using SpecBox.WebApi.Lib.Logging;
using SpecBox.WebApi.Model;

var builder = WebApplication.CreateBuilder(args);

string? cstring = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<SpecBoxDbContext>(cfg => cfg.UseNpgsql(cstring));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonDateTimeUTCConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());

builder.Services.AddSwaggerGen(opts =>
{
    opts.CustomSchemaIds(a => a.FullName);
    opts.SupportNonNullableReferenceTypes();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddConsoleFormatter<ConsoleJsonFormatter, ConsoleFormatterOptions>();

var app = builder.Build();
// app.UsePathBase(app.Configuration["pathBase"]);
app.UsePathBase("/api");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
