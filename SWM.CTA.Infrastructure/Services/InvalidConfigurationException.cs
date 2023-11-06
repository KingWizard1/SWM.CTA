using System;
using System.ComponentModel.DataAnnotations;

namespace SWM.CTA.Infrastructure.Services;

/// <summary>Thrown when a required service configuration object has been determined to be invalid.</summary>
public class InvalidConfigurationException : Exception
{
    public InvalidConfigurationException(Type configurationType) : 
        base($"The service configuration of type '{configurationType.Name}' is not valid") { }

    public InvalidConfigurationException(Type configurationType, ValidationResult validationResult) :
        base(string.Format("Invalid configuration value for members '{0}' on object {1}: {2}",
            string.Join(", ", validationResult.MemberNames), configurationType.Name, validationResult.ErrorMessage ?? "No reason provided")) { }
}