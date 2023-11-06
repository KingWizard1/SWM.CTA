using System;

namespace SWM.CTA.Infrastructure.Services;

/// <summary>Thrown when a required service configuration object could not be loaded because it is missing.</summary>
public class MissingServiceConfigurationException : Exception
{
    public MissingServiceConfigurationException(Type configurationType) : 
        base($"Cannot register configuration object of type '{configurationType.Name}'. The required configuration is could not be found.") { }
    
}