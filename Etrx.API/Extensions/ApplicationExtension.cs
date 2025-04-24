using Etrx.Application.Interfaces;
using Etrx.Application.Services;

namespace Etrx.API.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProblemsService, ProblemsService>();
        services.AddScoped<IContestsService, ContestsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISubmissionsService, SubmissionsService>();
        services.AddScoped<IRanklistRowsService, RanklistRowsService>();
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
