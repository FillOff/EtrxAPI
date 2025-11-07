using Etrx.Domain.Interfaces;
using Etrx.Domain.Interfaces.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Repositories;
using Etrx.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Etrx.API.Extensions;

public static class PersistenceExtension
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
}
