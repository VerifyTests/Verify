#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        public Task Verify(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }
    }
}
#endif