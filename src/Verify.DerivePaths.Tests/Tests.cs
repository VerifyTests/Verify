﻿using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    static Tests()
    {
        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                Assert.True(File.Exists(sourceFile));
                Assert.True(Directory.Exists(projectDirectory));
                Assert.NotNull(method);
                Assert.NotNull(type);
                Assert.EndsWith("Verify.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                Assert.EndsWith("Verify.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new PathInfo("CustomDir", "CustomTypeName", "CustomMethodName");
            });
    }

    [Fact]
    public async Task ThrowOnConflict()
    {
        static Task Run()
        {
            return Verifier.Verify("Value")
                .UseMethodName("Conflict")
                .DisableDiff();
        }

        try
        {
            await Run();
        }
        catch
        {
        }

        await Verifier.ThrowsTask(Run)
            .UseMethodName("ThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }

    [Fact]
    public async Task Test()
    {
        await Verifier.Verify("Value");
    }
}