using System.IO;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        (string receivedPath, string verifiedPath) GetFileNames()
        {
            var filePrefix = Path.Combine(SourceDirectory, Context.UniqueTestName);
            var receivedPath = $"{filePrefix}.received{extension}";
            var verifiedPath = $"{filePrefix}.verified{extension}";
            return (receivedPath, verifiedPath);
        }

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }
    }
}