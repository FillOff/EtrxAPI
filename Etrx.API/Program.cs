using Etrx.API.Extensions;
using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProfiles();
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();
builder.Services.AddDbContexts();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer 
                { 
                    Url = $"https://dl.gsu.by/etrx2",
                    Description = "For swagger in production"
                },
                new OpenApiServer 
                { 
                    Url = $"{httpReq.Scheme}://{httpReq.Host}",
                    Description = "For swagger in development"
                }
            };
        });
    });
 
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.UseCors(options =>
{
    options.WithHeaders().AllowAnyHeader();
    options.WithOrigins().AllowAnyOrigin();
    options.WithMethods().AllowAnyMethod();
});

app.Run();