using Etrx.API.Profiles;

namespace Etrx.API.Extensions;

public static class AutoMapperExtension
{
    public static IServiceCollection AddProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(ProblemsProfile),
            typeof(ContestsProfile),
            typeof(UsersProfile));

        return services;
    }
}
