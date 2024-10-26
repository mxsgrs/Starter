using Microsoft.OpenApi.Models;

namespace Starter.WebApi.Utilities;

public static class CustomSwagger
{
    private static string _version = "v1";

    public static void AddCustomSwaggerGen(this WebApplicationBuilder builder)
    {
        _version = builder.Configuration.GetValue<string>("Version")
            ?? throw new Exception("Version number is missing");

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(_version, new OpenApiInfo
            {
                Version = _version,
                Title = $"Starter.WebApi",
                Description = "Get your starter web API."
            });
        });
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/api/swagger/{_version}/swagger.json", _version);
            options.RoutePrefix = $"api/swagger";
        });
    }
}
