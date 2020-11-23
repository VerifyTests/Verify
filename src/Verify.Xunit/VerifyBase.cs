using System.Runtime.CompilerServices;
using VerifyTests;

namespace VerifyXunit
{
    [UsesVerify]
    public abstract partial class VerifyBase
    {
        VerifySettings? settings;
        string sourceFile;

        public VerifyBase(VerifySettings? settings = null, [CallerFilePath] string sourceFile = "")
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
            {
                throw new($"{nameof(VerifyBase)}.ctor must be called explicitly.");
            }

            this.settings = settings;
            this.sourceFile = sourceFile;
        }
    }
}