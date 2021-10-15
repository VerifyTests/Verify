namespace VerifyTests;

public partial class VerifySettings
{
    internal Namer Namer = new();
    
    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public void UniqueForRuntime()
    {
        Namer.UniqueForRuntime = true;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public void UniqueForTargetFramework()
    {
        Namer.UniqueForTargetFramework = true;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public void UniqueForTargetFrameworkAndVersion()
    {
        Namer.UniqueForTargetFrameworkAndVersion = true;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public void UniqueForAssemblyConfiguration()
    {
        Namer.UniqueForAssemblyConfiguration = true;
    }

    /// <summary>
    /// Use <paramref name="assembly"/> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public void UniqueForTargetFramework(Assembly assembly)
    {
        Namer.UniqueForTargetFramework = true;
        Namer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public void UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        Namer.UniqueForTargetFrameworkAndVersion = true;
        Namer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public void UniqueForAssemblyConfiguration(Assembly assembly)
    {
        Namer.UniqueForAssemblyConfiguration = true;
        Namer.UniqueForAssemblyConfigurationAssembly = assembly;
    }

    public string? Directory { get; internal set; }

    /// <summary>
    /// Use a custom directory for the test results.
    /// </summary>
    public void UseDirectory(string directory)
    {
        Guard.BadDirectoryName(directory, nameof(directory));
        Directory = directory;
    }

    internal string? typeName;

    /// <summary>
    /// Use a custom class name for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
    public void UseTypeName(string name)
    {
        Guard.BadFileName(name, nameof(name));
        ThrowIfFileNameDefined();

        typeName = name;
    }

    internal string? methodName;

    /// <summary>
    /// Use a custom method name for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
    public void UseMethodName(string name)
    {
        Guard.BadFileName(name, nameof(name));
        ThrowIfFileNameDefined();

        methodName = name;
    }

    internal string? fileName;

    /// <summary>
    /// Use a file name for the test results.
    /// Overrides the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file naming.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseTypeName"/>, <see cref="UseMethodName"/>, or <see cref="UseParameters"/>.</remarks>
    public void UseFileName(string fileName)
    {
        Guard.BadFileName(fileName, nameof(fileName));
        ThrowIfMethodOrTypeNameDefined();

        this.fileName = fileName;
    }

    void ThrowIfMethodOrTypeNameDefined()
    {
        if (methodName != null ||
            typeName is not null)
        {
            throw new($"{nameof(UseFileName)} is not compatible with {nameof(UseMethodName)} or {nameof(UseTypeName)}.");
        }
    }

    void ThrowIfFileNameDefined([CallerMemberName] string caller = "")
    {
        if (fileName is not null)
        {
            throw new($"{caller} is not compatible with {nameof(UseFileName)}.");
        }
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    public void UniqueForRuntimeAndVersion()
    {
        Namer.UniqueForRuntimeAndVersion = true;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public void UniqueForArchitecture()
    {
        Namer.UniqueForArchitecture = true;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public void UniqueForOSPlatform()
    {
        Namer.UniqueForOSPlatform = true;
    }

    internal bool UniquePrefixDisabled;

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public void DisableRequireUniquePrefix()
    {
        UniquePrefixDisabled = true;
    }
}