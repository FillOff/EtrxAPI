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
        services.AddScoped<ICodeforcesApiService, CodeforcesApiService>();
        services.AddScoped<IDlApiService, DlApiService>();

        services.AddSingleton<ILastUpdateTimeService, LastUpdateTimeService>();
        services.AddSingleton<IUpdateDataService, UpdateDataService>();

        services.AddHostedService<UpdateDataService>();

        services.AddHttpClient<IApiService, ApiService>();

        return services;
    }
}
