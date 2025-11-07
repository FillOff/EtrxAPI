using Microsoft.OpenApi.Models;

namespace Etrx.API.Extensions;

public static class SwaggerExtensions
{
    public static void ConfigureSwagger(this WebApplication? app)
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
            {
                new() {
                    Url = $"https://dl.gsu.by/etrx2",
                    Description = "For swagger in production"
                },
                new() {
                    Url = $"{httpReq.Scheme}://{httpReq.Host}",
                    Description = "For swagger in development"
                }
            };
            });
        });

        app.UseSwaggerUI();
    }
}
