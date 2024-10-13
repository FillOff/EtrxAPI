using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Persistence;
using Etrx.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EtrxDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(EtrxDbContext)));
    });

builder.Services.AddScoped<IProblemsService, ProblemsService>();
builder.Services.AddScoped<IProblemsRepository, ProblemsRepository>();
builder.Services.AddScoped<IContestsService, ContestsService>();
builder.Services.AddScoped<IContestsRepository, ContestsRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IJsonService, JsonService>();
builder.Services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();
builder.Services.AddScoped<ISubmissionsService, SubmissionsService>();

var app = builder.Build();

//app.Environment.EnvironmentName = "Production";

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(options =>
{
    options.WithHeaders().AllowAnyHeader();
    options.WithOrigins().AllowAnyOrigin();
    options.WithMethods().AllowAnyMethod();
});

app.Run();
