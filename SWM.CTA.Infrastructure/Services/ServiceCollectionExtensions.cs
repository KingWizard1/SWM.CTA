using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SWM.CTA.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    
    public static IServiceCollection ValidateAndAddServiceOptions<T>(this IServiceCollection services, IConfiguration config) where T : class
    {
        
        T? settings = config.GetRequiredSection(typeof(T).Name).Get<T>();

        if (settings == null)
            throw new MissingServiceConfigurationException(typeof(T));
        
        // TODO
        //settings.Validate();

        services.Configure<T>(config.GetRequiredSection(typeof(T).Name));

        return services;
    }

}