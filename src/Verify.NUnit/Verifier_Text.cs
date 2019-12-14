using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Verify;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task Verify(
            string target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(target, settings);
        }
    }
}