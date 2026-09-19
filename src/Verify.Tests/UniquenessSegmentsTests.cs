#pragma warning disable VerifyDanglingSnapshots

/// <summary>
/// Written against the values <c>Namer</c> reports for the run executing them, rather than against
/// literals, so the same assertions hold on every target framework, OS and architecture.
/// </summary>
public class UniquenessSegmentsTests
{
    // A runtime name that is never the one this run produces.
    static readonly string foreignRuntime = Namer.Runtime == "Mono" ? "DotNet" : "Mono";

    static readonly string foreignRuntimeAndVersion = $"{foreignRuntime}3_1";

    static readonly string foreignPlatform = Namer.OperatingSystemPlatform == "Linux" ? "Windows" : "Linux";

    static readonly string foreignArchitecture = Namer.Architecture == "arm64" ? "x64" : "arm64";

    [Fact]
    public void NoUniquenessIsNotAnotherRun()
    {
        Assert.False(BelongsToAnotherRun(".verified.txt"));
        Assert.False(BelongsToAnotherRun("_param=value.verified.txt"));
        Assert.False(BelongsToAnotherRun("#00.verified.txt"));
        Assert.False(BelongsToAnotherRun("#name.verified.txt"));
    }

    [Fact]
    public void ThisRuntimeIsNotAnotherRun()
    {
        Assert.False(BelongsToAnotherRun($".{Namer.Runtime}.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{Namer.RuntimeAndVersion}.verified.txt"));
    }

    [Fact]
    public void AnotherRuntimeIsAnotherRun()
    {
        Assert.True(BelongsToAnotherRun($".{foreignRuntime}.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}.verified.txt"));
    }

    /// <summary>
    /// A version on the same runtime as this run, but not this run's version. The version has to be
    /// compared, not just the runtime name, or every framework of a multi targeted project would
    /// look like this one.
    /// </summary>
    [Fact]
    public void AnotherVersionOfThisRuntimeIsAnotherRun()
    {
        var other = $"{Namer.Runtime}99_9";

        Assert.NotEqual(Namer.RuntimeAndVersion, other);
        Assert.True(BelongsToAnotherRun($".{other}.verified.txt"));
    }

    /// <summary>
    /// The manifests settle the runtime and framework axis, so once they cover every framework the
    /// segments naming it say nothing.
    /// </summary>
    [Fact]
    public void RuntimeIsIgnoredOnceFrameworksAreCovered() =>
        Assert.False(BelongsToAnotherRun($".{foreignRuntimeAndVersion}.verified.txt", frameworksCovered: true));

    [Fact]
    public void ThisPlatformIsNotAnotherRun() =>
        Assert.False(BelongsToAnotherRun($".{Namer.OperatingSystemPlatform}.verified.txt"));

    /// <summary>
    /// No manifest can settle the OS axis: those runs are on other machines, with their own
    /// intermediate directories. So a foreign platform stays foreign even when covered.
    /// </summary>
    [Fact]
    public void AnotherPlatformIsAnotherRunEvenWhenCovered()
    {
        Assert.True(BelongsToAnotherRun($".{foreignPlatform}.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{foreignPlatform}.verified.txt", frameworksCovered: true));
    }

    [Fact]
    public void ThisArchitectureIsNotAnotherRun() =>
        Assert.False(BelongsToAnotherRun($".{Namer.Architecture}.verified.txt"));

    [Fact]
    public void AnotherArchitectureIsAnotherRunEvenWhenCovered()
    {
        Assert.True(BelongsToAnotherRun($".{foreignArchitecture}.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{foreignArchitecture}.verified.txt", frameworksCovered: true));
    }

    /// <summary>
    /// The configuration axis has no enumerable value set, so a segment naming one is not treated as
    /// uniqueness at all. A snapshot only another configuration produces is reported.
    /// </summary>
    [Fact]
    public void ConfigurationIsNotRecognised()
    {
        Assert.True(!BelongsToAnotherRun(".Debug.verified.txt"));
        Assert.True(!BelongsToAnotherRun(".Release.verified.txt"));
    }

    /// <summary>
    /// A whole segment match, so a name that merely starts a word with a uniqueness token is left
    /// alone. These are what the old substring list could not tell apart.
    /// </summary>
    [Fact]
    public void NamesStartingWithAUniquenessTokenAreNotUniqueness()
    {
        Assert.False(BelongsToAnotherRun(".NetworkClient.verified.txt"));
        Assert.False(BelongsToAnotherRun(".NetCoreShim.verified.txt"));
        Assert.False(BelongsToAnotherRun(".Nettle.verified.txt"));
        Assert.False(BelongsToAnotherRun(".Windowsill.verified.txt"));
        Assert.False(BelongsToAnotherRun(".Linuxish.verified.txt"));
    }

    [Fact]
    public void CaseIsPartOfTheMatch()
    {
        Assert.False(BelongsToAnotherRun(".windows.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{foreignRuntime.ToLowerInvariant()}.verified.txt"));
    }

    /// <summary>
    /// The first segment is the parameter text, which is never uniqueness. A parameter value that
    /// happens to read like a runtime must not be mistaken for one.
    /// </summary>
    [Fact]
    public void ParametersAreNotUniqueness() =>
        Assert.False(BelongsToAnotherRun($"_param={foreignRuntimeAndVersion}.verified.txt"));

    [Fact]
    public void UniquenessAfterParameters() =>
        Assert.True(BelongsToAnotherRun($"_param=value.{foreignRuntimeAndVersion}.verified.txt"));

    [Fact]
    public void UniquenessBeforeAnIndex()
    {
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}#00.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}#name.verified.txt"));
    }

    [Fact]
    public void SeveralUniquenessSegments()
    {
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}.{Namer.OperatingSystemPlatform}.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{Namer.RuntimeAndVersion}.{foreignPlatform}.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{Namer.RuntimeAndVersion}.{Namer.OperatingSystemPlatform}.verified.txt"));
    }

    /// <summary>
    /// The unique directory conventions put the uniqueness in a directory name. Uniqueness ends
    /// where that directory does, so the file name below it is never read as uniqueness.
    /// </summary>
    [Fact]
    public void UniqueDirectory()
    {
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}{Path.DirectorySeparatorChar}target.verified.txt"));
        Assert.False(BelongsToAnotherRun($"{Path.DirectorySeparatorChar}{foreignRuntimeAndVersion}.verified.txt"));
    }

    /// <summary>
    /// Split mode names the directory <c>{prefix}{uniqueness}.verified</c>, so uniqueness ends at
    /// the verified marker even though it is a directory rather than a file.
    /// </summary>
    [Fact]
    public void SplitModeUniqueDirectory()
    {
        Assert.True(BelongsToAnotherRun($".{foreignRuntimeAndVersion}.verified{Path.DirectorySeparatorChar}target.txt"));
        Assert.False(BelongsToAnotherRun($".verified{Path.DirectorySeparatorChar}target.txt"));
    }

    /// <summary>
    /// Only <c>{name}{major}_{minor}</c> is a versioned segment. Anything else keeps its underscores
    /// and fails the name lookup whole.
    /// </summary>
    [Fact]
    public void OnlyMajorMinorIsAVersion()
    {
        Assert.False(BelongsToAnotherRun($".{foreignRuntime}_.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{foreignRuntime}_x.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{foreignRuntime}3_.verified.txt"));
        Assert.False(BelongsToAnotherRun($".{foreignRuntime}_1.verified.txt"));
        Assert.True(BelongsToAnotherRun($".{foreignRuntime}10_11.verified.txt"));
    }

    static bool BelongsToAnotherRun(string tail, bool frameworksCovered = false) =>
        UniquenessSegments.BelongsToAnotherRun(tail, frameworksCovered);
}
