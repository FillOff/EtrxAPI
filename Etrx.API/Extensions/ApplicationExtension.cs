using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;

namespace Etrx.API.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProblemsService, ProblemsService>();
        services.AddScoped<IContestsService, ContestsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISubmissionsService, SubmissionsService>();
        services.AddScoped<ICodeforcesService, CodeforcesService>();

        services.AddSingleton<ILastUpdateTimeService, LastUpdateTimeService>();
        services.AddSingleton<IUpdateDataService, UpdateDataService>();

        return services;
    }
}
