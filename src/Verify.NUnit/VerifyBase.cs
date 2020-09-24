using System.Runtime.CompilerServices;
using NUnit.Framework;
using VerifyTests;

namespace VerifyNUnit
{
    [TestFixture]
    public partial class VerifyBase
    {
        VerifySettings? settings;
        string sourceFile;

        public VerifyBase(VerifySettings? settings = null, [CallerFilePath] string sourceFile = "")
        {
            this.settings = settings;
            this.sourceFile = sourceFile;
        }
    }
}