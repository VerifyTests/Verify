using System.Diagnostics;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class NamerTests
{
    [Fact]
    public Task Runtime()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntime();
        return Verifier.Verify(Namer.Runtime, settings);
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify(Namer.RuntimeAndVersion, settings);
    }

    [Fact]
    public async Task UseFileName()
    {
        #region UseFileName

        VerifySettings settings = new();
        settings.UseFileName("CustomFileName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public Task UseFileNameWithUnique()
    {
        VerifySettings settings = new();
        settings.UseFileName("CustomFileName");
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public async Task UseFileNameFluent()
    {
        #region UseFileNameFluent

        await Verifier.Verify("value")
            .UseFileName("CustomFileNameFluent");

        #endregion
    }

    [Fact]
    public async Task UseDirectory()
    {
        #region UseDirectory

        VerifySettings settings = new();
        settings.UseDirectory("CustomDirectory");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseDirectoryFluent()
    {
        #region UseDirectoryFluent

        await Verifier.Verify("value")
            .UseDirectory("CustomDirectory");

        #endregion
    }

    [Fact]
    public async Task UseTypeName()
    {
        #region UseTypeName

        VerifySettings settings = new();
        settings.UseTypeName("CustomTypeName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseTypeNameFluent()
    {
        #region UseTypeNameFluent

        await Verifier.Verify("value")
            .UseTypeName("CustomTypeName");

        #endregion
    }

    [Fact]
    public async Task UseMethodName()
    {
        #region UseMethodName

        VerifySettings settings = new();
        settings.UseMethodName("CustomMethodName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseMethodNameFluent()
    {
        #region UseMethodNameFluent

        await Verifier.Verify("value")
            .UseMethodName("CustomMethodNameFluent");

        #endregion
    }

    [Fact]
    public void AccessNamerRuntimeAndVersion()
    {
        #region AccessNamerRuntimeAndVersion
        Debug.WriteLine(Namer.Runtime);
        Debug.WriteLine(Namer.RuntimeAndVersion);
        #endregion
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        VerifySettings settings = new();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("Foo", settings);
    }
}