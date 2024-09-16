using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpecBox.Domain;
using SpecBox.WebApi.Lib.Logging;
using SpecBox.WebApi.Model;
using SpecBox.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

string? cstring = builder.Configuration.GetConnectionString("default");

builder.Services.AddDbContext<SpecBoxDbContext>(cfg => cfg.UseNpgsql(cstring));

builder.Services.AddCors();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonDateTimeUTCConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<AuthService>();

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    string privateKey = builder.Configuration["PrivateKey"] ?? "MySuperSecretPrivateKeyWithLengthMoreThan128bits";
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AuthProfile>());

builder.Services.AddSwaggerGen(opts =>
{
    opts.CustomSchemaIds(a => a.FullName);
    opts.SupportNonNullableReferenceTypes();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    opts.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            new string[] {}
        }
    });
});

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddConsoleFormatter<ConsoleJsonFormatter, ConsoleFormatterOptions>();

var app = builder.Build();

app.UseCors(opts =>
{
    opts.AllowAnyOrigin();
    opts.AllowAnyHeader();
    opts.AllowAnyMethod();
});

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase(app.Configuration["pathBase"]);
app.UseAuthentication();
app.UseAuthorization();

app.Run();
