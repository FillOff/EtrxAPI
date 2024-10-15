using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Etrx.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProblemsService, ProblemsService>();
            services.AddScoped<IContestsService, ContestsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IJsonService, JsonService>();
            services.AddScoped<ISubmissionsService, SubmissionsService>();

            return services;
        }
    }
}
