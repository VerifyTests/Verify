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
            settings.Context["Parameters"] = parameters;
        }

        public static SettingsTask UseParameters(this SettingsTask settings, params object?[] parameters)
        {
            settings.CurrentSettings.UseParameters(parameters);
            return settings;
        }

        public static SettingsTask GetParameters(this SettingsTask settings, MethodInfo methodInfo)
        {
            settings.CurrentSettings.GetParameters(methodInfo);
            return settings;
        }

        public static object?[] GetParameters(this VerifySettings settings, MethodInfo methodInfo)
        {
            var settingsParameters = settings.ParametersOrDefault();
            var methodParameters = methodInfo.GetParameters();
            if (!methodParameters.Any() || settingsParameters.Any())
            {
                return settingsParameters;
            }

            var names = string.Join(", ", methodParameters.Select(x => x.Name));
            throw InnerVerifier.exceptionBuilder($@"Method `{methodInfo.DeclaringType!.Name}.{methodInfo.Name}` requires parameters, but none have been defined. Add UseParameters. For example:

VerifySettings settings = new();
settings.UseParameters({names});
await Verifier.Verify(target, settings);

or

await Verifier.Verify(target).UseParameters({names});
");
        }

        static object?[] ParametersOrDefault(this VerifySettings settings)
        {
            if (settings.Context.TryGetValue("Parameters", out var data))
            {
                return (object?[]) data;
            }

            return Array.Empty<object?>();
        }
    }
}