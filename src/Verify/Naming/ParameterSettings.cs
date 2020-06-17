using System;
using System.Linq;
using System.Reflection;

namespace VerifyTests
{
    public static class ParameterSettings
    {
        public static void UseParameters(this VerifySettings settings, params object?[] parameters)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNullOrEmpty(parameters, nameof(parameters));
            settings.Data["Parameters"] = parameters;
        }

        public static object?[] GetParameters(this VerifySettings? settings, MethodInfo methodInfo)
        {
            var settingsParameters = settings.ParametersOrDefault();
            var methodParameters = methodInfo.GetParameters();
            if (!methodParameters.Any() || settingsParameters.Any())
            {
                return settingsParameters;
            }
            throw InnerVerifier.exceptionBuilder($@"Method `{methodInfo.DeclaringType.Name}.{methodInfo.Name}` requires parameters, but none have been defined. Add UseParameters. For example:
var settings = new VerifySettings();
settings.UseParameters({string.Join(", ", methodParameters.Select(x => x.Name))});
await Verifier.Verify(target, settings);");

        }
        static object?[] ParametersOrDefault(this VerifySettings? settings)
        {
            if (settings == null)
            {
                return Array.Empty<object?>();
            }

            if (settings.Data.TryGetValue("Parameters", out var data))
            {
                return (object?[]) data;
            }
            return Array.Empty<object?>();
        }
    }
}