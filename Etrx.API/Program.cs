using Etrx.API.Extensions;
using Etrx.API.Middlewares;
using Serilog;
using Etrx.API.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.AddSerilog();

builder.Services.ConfigureControllers();
builder.Services.AddEtrxAuthentication(builder.Environment.IsDevelopment());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddServices();
builder.Services.AddHttpClient<Etrx.Application.Interfaces.ICodeforcesProgressClient, Etrx.API.Services.CodeforcesProgressClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(45));
builder.Services.AddBackgroundServices();
builder.Services.AddRepositories();
builder.Services.ConfigureOptions(configuration);
builder.Services.AddDbContexts(configuration);
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.ConfigureSwagger();
}

await app.ApplyMigrationsAsync();

app.UseFrontendCors();
app.UseApiRequestHeader();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();
