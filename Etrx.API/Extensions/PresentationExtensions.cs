using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

namespace Etrx.API.Extensions;

public static class PresentationExtensions
{
    public static IHostBuilder AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, configuration) => configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .WriteTo.Console());

        return host;
    }

    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

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
