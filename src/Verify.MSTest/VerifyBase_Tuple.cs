#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        [Obsolete("Use VerifyTuple")]
        public SettingsTask Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyTuple(expression, settings, sourceFile);
        }

        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyTuple(target));
        }
    }
}
#endif