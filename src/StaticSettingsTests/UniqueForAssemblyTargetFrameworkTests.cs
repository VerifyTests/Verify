public class UniqueForAssemblyTargetFrameworkTests :
    BaseTest
{
    // The static UniqueForTargetFramework*(Assembly) overloads store the framework name on
    // the shared namer, so the uniqueness builder must read it through the resolver rather
    // than only the per-test namer. The name is overwritten with a synthetic one so the
    // assertion cannot accidentally pass via the fallback to the test assembly's own
    // TargetFrameworkAttribute.
    [Fact]
    public void AssemblyOverloadFeedsTheSharedNamer()
    {
        var sharedNamer = VerifierSettings.SharedNamer;
        try
        {
            VerifierSettings.UniqueForTargetFrameworkAndVersion(typeof(UniqueForAssemblyTargetFrameworkTests).Assembly);
            sharedNamer.UniqueForTargetFrameworkName = new("FakeFx", "FakeFx9_9");

            var uniqueness = PrefixUnique.SharedUniqueness(new());

            Assert.Equal(".FakeFx9_9", uniqueness.ToString());
        }
        finally
        {
            sharedNamer.UniqueForTargetFrameworkAndVersion = false;
            sharedNamer.UniqueForTargetFrameworkName = null;
        }
    }

    [Fact]
    public void AssemblyOverloadFeedsTheSharedNamerWithoutVersion()
    {
        var sharedNamer = VerifierSettings.SharedNamer;
        try
        {
            VerifierSettings.UniqueForTargetFramework(typeof(UniqueForAssemblyTargetFrameworkTests).Assembly);
            sharedNamer.UniqueForTargetFrameworkName = new("FakeFx", "FakeFx9_9");

            var uniqueness = PrefixUnique.SharedUniqueness(new());

            Assert.Equal(".FakeFx", uniqueness.ToString());
        }
        finally
        {
            sharedNamer.UniqueForTargetFramework = false;
            sharedNamer.UniqueForTargetFrameworkName = null;
        }
    }
}
