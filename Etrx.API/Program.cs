using Etrx.API.Extensions;
using Etrx.API.Middlewares;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddDbContexts(builder.Configuration);
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.ConfigureSwagger();
}

await app.ApplyMigrationsAsync();

app.UseAuthorization();
app.MapControllers();

app.UseCors(options =>
{
    options.WithHeaders().AllowAnyHeader();
    options.WithOrigins().AllowAnyOrigin();
    options.WithMethods().AllowAnyMethod();
});

app.Run();