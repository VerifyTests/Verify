using System;
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
            if ((settings == null) && (sourceFile == ""))
                throw new InvalidOperationException($"{nameof(VerifyBase)}.ctor must be called explicitly.");

            this.settings = settings;
            this.sourceFile = sourceFile;
        }
    }
}