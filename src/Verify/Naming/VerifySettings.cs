namespace VerifyTests;

public partial class VerifySettings
{
    internal Namer Namer = new();

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public VerifySettings UniqueForRuntime()
    {
        Namer.UniqueForRuntime = true;
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public VerifySettings UniqueForTargetFramework()
    {
        Namer.UniqueForTargetFramework = true;
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public VerifySettings UniqueForTargetFrameworkAndVersion()
    {
        Namer.UniqueForTargetFrameworkAndVersion = true;
        return this;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public VerifySettings UniqueForAssemblyConfiguration()
    {
        Namer.UniqueForAssemblyConfiguration = true;
        return this;
    }

    /// <summary>
    /// Use <paramref name="assembly" /> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public VerifySettings UniqueForTargetFramework(Assembly assembly)
    {
        Namer.UniqueForTargetFramework = true;
        Namer.SetUniqueForAssemblyFrameworkName(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public VerifySettings UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        Namer.UniqueForTargetFrameworkAndVersion = true;
        Namer.SetUniqueForAssemblyFrameworkName(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public VerifySettings UniqueForAssemblyConfiguration(Assembly assembly)
    {
        Namer.UniqueForAssemblyConfiguration = true;
        Namer.SetUniqueForAssemblyConfiguration(assembly);
        return this;
    }

    public string? Directory { get; internal set; }

    /// <summary>
    /// Use a custom directory for the test results.
    /// </summary>
    public VerifySettings UseDirectory(string directory)
    {
        Guards.BadDirectoryName(directory);
        Directory = directory;
        return this;
    }

    internal string? typeName;

    /// <summary>
    /// Use a custom class name for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName" />.</remarks>
    public VerifySettings UseTypeName(string name)
    {
        Guards.BadFileName(name);
        ThrowIfFileNameDefined();

        typeName = name;
        return this;
    }

    internal string? methodName;

    /// <summary>
    /// Use a custom method name for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName" />.</remarks>
    public VerifySettings UseMethodName(string name)
    {
        Guards.BadFileName(name);
        ThrowIfFileNameDefined();

        methodName = name;
        return this;
    }

    void ThrowIfFileNameDefined([CallerMemberName] string caller = "")
    {
        if (fileName is not null)
        {
            throw new($"{caller} is not compatible with {nameof(UseFileName)}.");
        }
    }

    internal string? fileName;

    /// <summary>
    /// Use a file name for the test results.
    /// Overrides the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file naming.
    /// Where the new file format is `{CurrentDirectory}/{FileName}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseTypeName" />, <see cref="UseMethodName" />, or <see cref="UseParameters" />.</remarks>
    public VerifySettings UseFileName(string fileName)
    {
        Guards.BadFileName(fileName);
        ThrowIfMethodOrTypeNameDefined();

        this.fileName = fileName;
        return this;
    }

    void ThrowIfMethodOrTypeNameDefined()
    {
        if (methodName is not null ||
            typeName is not null)
        {
            throw new($"{nameof(UseFileName)} is not compatible with {nameof(UseMethodName)} or {nameof(UseTypeName)}.");
        }
    }

    internal bool useUniqueDirectory;

    /// <summary>
    /// Use a directory for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}/{targetName}.verified.{extension}`.
    /// </summary>
    public VerifySettings UseUniqueDirectory()
    {
        useUniqueDirectory = true;
        return this;
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    public VerifySettings UniqueForRuntimeAndVersion()
    {
        Namer.UniqueForRuntimeAndVersion = true;
        return this;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public VerifySettings UniqueForArchitecture()
    {
        Namer.UniqueForArchitecture = true;
        return this;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public VerifySettings UniqueForOSPlatform()
    {
        Namer.UniqueForOSPlatform = true;
        return this;
    }

    internal bool UniquePrefixDisabled;

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public VerifySettings DisableRequireUniquePrefix()
    {
        UniquePrefixDisabled = true;
        return this;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public VerifySettings UseSplitModeForUniqueDirectory()
    {
        UseUniqueDirectorySplitMode = true;
        return this;
    }

    /// <summary>
    /// Dont use the current runtime to make the test results unique.
    /// Overrides <see cref="VerifierSettings.UseSplitModeForUniqueDirectory" />.
    /// </summary>
    public VerifySettings DontUseSplitModeForUniqueDirectory()
    {
        UseUniqueDirectorySplitMode = false;
        return this;
    }

    internal bool? UseUniqueDirectorySplitMode;
}