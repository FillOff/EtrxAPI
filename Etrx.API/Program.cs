using Etrx.Application.Services;
using Etrx.Core.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Persistence;
using Etrx.Persistence.DbProfiles;
using Etrx.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ProblemsProfile));

builder.Services.AddDbContext<EtrxDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(EtrxDbContext)));
    });

builder.Services.AddScoped<IProblemsService, ProblemsService>();
builder.Services.AddScoped<IProblemsRepository, ProblemsRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
