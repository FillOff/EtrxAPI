using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Repositories;

namespace Etrx.API.Extensions;

public static class PersistenceExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProblemsRepository, ProblemsRepository>();
        services.AddScoped<IContestsRepository, ContestsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();

        return services;
    }

    public static IServiceCollection AddDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<EtrxDbContext>();

        return services;
    }
}
