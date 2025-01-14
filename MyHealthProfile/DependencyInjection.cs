using AutoMapper;
using DoodiServicesShopingCartAPI;
using MyHealthProfile.Repositories.Account;
using MyHealthProfile.Services;
using MyHealthProfile.Services.Interfaces;

namespace MyHealthProfile
{
    public static class DependencyInjection
    {
        internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityService, IdentityService>();

            IMapper mapper = MappingConfig.RegisterMap().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            return services;
        }
    }
}
