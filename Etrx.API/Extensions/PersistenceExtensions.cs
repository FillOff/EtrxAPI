using Etrx.Application.Repositories;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Repositories;
using Etrx.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Etrx.API.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProblemsRepository, ProblemsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IContestsRepository, ContestsRepository>();
        services.AddScoped<IContestTranslationsRepository, ContestTranslationsRepository>();
        services.AddScoped<IProblemTranslationsRepository, ProblemTranslationsRepository>();
        services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();
        services.AddScoped<IRanklistRowsRepository, RanklistRowsRepository>();
        services.AddScoped<IGenericRepository<ProblemResult>, GenericRepository<ProblemResult>>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EtrxDbContext>(options =>
            options
                .UseNpgsql(configuration["ETRX_DB_CONNECTION_STRING"])
                .UseSnakeCaseNamingConvention());

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IHost app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(PersistenceExtensions));

        try
        {
            var dbContext = serviceProvider.GetRequiredService<EtrxDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }
    }
}
