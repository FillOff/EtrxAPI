using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Etrx.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProblemsRepository, ProblemsRepository>();
            services.AddScoped<IContestsRepository, ContestsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();

            return services;
        }
    }
}
