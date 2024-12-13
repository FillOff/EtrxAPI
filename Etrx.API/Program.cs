using Etrx.API.Profiles;
using Etrx.Application;
using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Etrx.Persistence;
using Etrx.Persistence.Databases;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddAutoMapper(typeof(ProblemsProfile), 
                               typeof(ContestsProfile), 
                               typeof(UsersProfile));

builder.Services.AddDbContext<EtrxDbContext>();

builder.Services.AddHttpClient<ICodeforcesApiService, CodeforcesApiService>();
builder.Services.AddHostedService<UpdateDataService>();
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();

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
                    Url = $"{httpReq.Scheme}://{httpReq.Host}/etrx2",
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