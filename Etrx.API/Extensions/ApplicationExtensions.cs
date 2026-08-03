using Etrx.Application.Interfaces;
using Etrx.Application.Interfaces.Api;
using Etrx.Application.Options;
using Etrx.Application.Services;
using Etrx.Application.Services.Api;
using Etrx.Application.Services.BackgroundServices;
using Etrx.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Net;

namespace Etrx.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<CookieContainer>();
        services.AddScoped<IProblemsService, ProblemsService>();
        services.AddScoped<IContestsService, ContestsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISubmissionsService, SubmissionsService>();
        services.AddScoped<IRanklistRowsService, RanklistRowsService>();
        services.AddScoped<ICodeforcesService, CodeforcesService>();
        services.AddScoped<IIoiCodeforcesService, IoiCodeforcesService>();
        services.AddScoped<ICodeforcesApiService, CodeforcesApiService>();
        services.AddScoped<IDlApiService, DlApiService>();
        services.AddScoped<IUpdateDataService, UpdateDataService>();
        services.AddScoped<ITagService, TagService>();
        services.AddHttpClient<IIoiCodeforcesApiService, IoiCodeforcesApiService>()
            .ConfigurePrimaryHttpMessageHandler(serviceProvider => new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = serviceProvider.GetRequiredService<CookieContainer>(),
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.All
            });

        services.AddHttpClient<ApiService>();

        services.AddValidatorsFromAssemblyContaining<GetSortUserRequestDtoValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<UpdateDataEvery30MinutesBackgroundService>();
        services.AddHostedService<UpdateDataPerDayBackgroundService>();

        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CodeforcesOptions>(
            configuration.GetSection(CodeforcesOptions.SectionName));

        return services;
    }
}
