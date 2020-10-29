#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static SettingsTask Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(expression));
        }
    }
}
#endif