using Etrx.API.Profiles;
using Etrx.Application;
using Etrx.Persistence;
using Etrx.Persistence.Databases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ProblemsProfile), 
                               typeof(ContestsProfile), 
                               typeof(UsersProfile));

builder.Services.AddDbContext<EtrxDbContext>();

builder.Services.AddApplicationServices();
builder.Services.AddRepositories();

var app = builder.Build();

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