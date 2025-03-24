﻿using Is = NUnit.Framework.Is;

[TestFixture]
public class Tests
{
    [Test]
    public Task Test()
    {
        VerifierSettings.Reset();
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                That(File.Exists(sourceFile));
                That(Directory.Exists(projectDirectory));
                That(methodName, Is.Not.Null);
                That(typeName, Is.Not.Null);
                // Assert.EndsWith("Verify.NUnit.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                // Assert.EndsWith("Verify.NUnit.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    [Test]
    public Task ReturnNulls()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [Test]
    public Task ProjectRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public Task SourceFileRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseSourceFileRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public void InvalidMethod()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public void InvalidType()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public void InvalidDirectory()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }
}