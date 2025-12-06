using Etrx.Application.Interfaces;
using Etrx.Application.Interfaces.Api;
using Etrx.Application.Services;
using Etrx.Application.Services.Api;
using Etrx.Application.Services.BackgroundServices;

namespace Etrx.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProblemsService, ProblemsService>();
        services.AddScoped<IContestsService, ContestsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISubmissionsService, SubmissionsService>();
        services.AddScoped<IRanklistRowsService, RanklistRowsService>();
        services.AddScoped<ICodeforcesService, CodeforcesService>();
        services.AddScoped<ICodeforcesApiService, CodeforcesApiService>();
        services.AddScoped<IDlApiService, DlApiService>();
        services.AddScoped<IUpdateDataService, UpdateDataService>();
        services.AddScoped<ITagService, TagService>();

        services.AddSingleton<ILastUpdateTimeService, LastUpdateTimeService>();

        services.AddHostedService<UpdateDataEvery30MinutesBackgroundService>();
        services.AddHostedService<UpdateDataPerDayBackgroundService>();

        services.AddHttpClient<IApiService, ApiService>();

        return services;
    }
}