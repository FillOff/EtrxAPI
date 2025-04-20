using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Interfaces;
using Etrx.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Etrx.API.Extensions;

public static class PersistenceExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProblemsRepository, ProblemsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IContestsRepository, ContestsRepository>();
        services.AddScoped<IGenericRepository<ContestTranslation, object>, GenericRepository<ContestTranslation, object>>();
        services.AddScoped<IGenericRepository<ProblemTranslation, object>, GenericRepository<ProblemTranslation, object>>();
        services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();

        return services;
    }

    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EtrxDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(nameof(EtrxDbContext))));

        return services;
    }
}
