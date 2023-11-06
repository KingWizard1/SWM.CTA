using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SWM.CTA.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>Read the given configuration object from its section in the provided <see cref="IConfiguration"/> and add it to the service collection.</summary>
    /// <param name="config">The configuration to read the object from. The configuration section is expected to be named the same as the configuration type.</param>
    /// <typeparam name="TSettings">The configuration type. The configuration section is expected to be named the same as the configuration type.</typeparam>
    /// <exception cref="MissingConfigurationException">Thrown if the requested service configuration is missing.</exception>
    public static IServiceCollection AddConfiguration<TSettings>(this IServiceCollection services, IConfiguration config)
        where TSettings : class
    {
        config.TryGetRequiredSection<TSettings>();
        services.Configure<TSettings>(config.GetRequiredSection(typeof(TSettings).Name));
        return services;
    }

    /// <summary>Attempts to read the given configuration object from its section in the provided <see cref="IConfiguration"/>.</summary>
    /// <typeparam name="TSettings">The configuration type. The configuration section is expected to be named the same as the configuration type.</typeparam>
    /// <exception cref="MissingConfigurationException">Thrown if the requested service configuration is missing.</exception>
    public static TSettings TryGetRequiredSection<TSettings>(this IConfiguration config) =>
        config.GetRequiredSection(typeof(TSettings).Name).Get<TSettings>() ?? 
        throw new MissingConfigurationException(typeof(TSettings));
    
    /// <summary>Read the given service configuration object from its section in the provided <see cref="IConfiguration"/>, validate it, and add it to the service collection.</summary>
    /// <param name="config">The configuration to read the object from. The configuration section is expected to be named the same as the configuration type.</param>
    /// <typeparam name="TSettings">The configuration type. The configuration section is expected to be named the same as the configuration type.</typeparam>
    /// <remarks>If the service configuration is missing or fails validation, an <see cref="InvalidConfigurationException"/> will be thrown containing details of what failed.</remarks>
    /// <exception cref="InvalidConfigurationException">Thrown if the requested service configuration is invalid.</exception>
    /// <exception cref="MissingConfigurationException">Thrown if the requested service configuration is missing.</exception>
    public static IServiceCollection AddServiceConfiguration<TSettings>(this IServiceCollection services, IConfiguration config) 
        where TSettings : class, IServiceConfiguration
    {
        // Try read settings of specified type
        TSettings settings = config.TryGetRequiredSection<TSettings>();

        // Try and validate the settings object
        List<ValidationResult> validationResults = new();
        ValidationContext validationContext = new(settings, null, null);
        Validator.TryValidateObject(settings, validationContext, validationResults, true);
        
        // Throw an exception if anything was invalid.
        // Program execution should not continue at this point.
        if (validationResults.Any())
            throw new InvalidConfigurationException(typeof(TSettings), validationResults.First());
        
        // Validation has passed. Add configuration to DI container.
        services.AddConfiguration<TSettings>(config);

        return services;
    }

}