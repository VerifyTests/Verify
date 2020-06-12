using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;

namespace VerifyMSTest
{
    public static class TestContextSettings
    {
        public static void UseTestContext(this VerifySettings settings, TestContext context)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNull(context, nameof(context));
            settings.Data["MsTestContext"] = context;
        }

        internal static bool TryGetTestContext(this VerifySettings? settings, [NotNullWhen(true)] out TestContext? context)
        {
            context = null;

            if (settings == null)
            {
                return false;
            }

            if (!settings.Data.TryGetValue("MsTestContext", out var contextObject))
            {
                return false;
            }
            context = (TestContext) contextObject;
            return true;

        }
    }
}