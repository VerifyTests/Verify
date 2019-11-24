using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }
    }
}