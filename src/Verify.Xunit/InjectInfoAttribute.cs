using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace VerifyXunit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectInfoAttribute :
        BeforeAfterTestAttribute
    {
        static AsyncLocal<MethodInfo?> local = new AsyncLocal<MethodInfo?>();

        public override void Before(MethodInfo info)
        {
            local.Value = info;
        }

        public override void After(MethodInfo info)
        {
            local.Value = null;
        }

        internal static bool TryGet([NotNullWhen(true)] out MethodInfo? info)
        {
            info = local.Value;
            if (info == null)
            {
                return false;
            }

            return true;
        }
    }
}