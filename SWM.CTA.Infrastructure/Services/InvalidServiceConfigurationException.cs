using System;

namespace SWM.CTA.Infrastructure.Services;

/// <summary>Thrown when a required service configuration object has been determined to be invalid.</summary>
public class InvalidServiceConfigurationException : Exception
{
    public InvalidServiceConfigurationException(Type serviceType, Type configurationType) : 
        base($"An instance of {serviceType.Name} cannot be constructed. Please check that the configuration instance of type '{configurationType.Name}' is valid.") { }
    
    public InvalidServiceConfigurationException(string message) : base(message) { }
    public InvalidServiceConfigurationException(string message, Exception ex) : base(message, ex) { }
}