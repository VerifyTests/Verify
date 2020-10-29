#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public SettingsTask Verify(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(target));
        }
    }
}
#endif