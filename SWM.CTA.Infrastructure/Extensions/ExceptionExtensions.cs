using System;

namespace SWM.CTA.Infrastructure.Extensions;

public static class ExceptionExtensions
{
    public static string ToInformativeString(this Exception ex) => $"{ex.GetType().Name}: {ex.Message}";
}