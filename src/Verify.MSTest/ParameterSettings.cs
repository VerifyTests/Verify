using System;
using Verify;

namespace VerifyMSTest
{
    public static class ParameterSettings
    {
        public static void UseParameters(this VerifySettings settings, params object[] parameters)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNullOrEmpty(parameters, nameof(parameters));
            settings.Data["Parameters"] = parameters;
        }

        internal static object[] GetParameters(this VerifySettings? settings)
        {
            if (settings == null)
            {
                return Array.Empty<object>();
            }

            if (settings.Data.TryGetValue("Parameters", out var data))
            {
                return (object[]) data;
            }

            return Array.Empty<object>();
        }
    }
}