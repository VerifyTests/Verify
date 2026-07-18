namespace VerifyTests;

public static partial class VerifierSettings
{
    /// <summary>
    /// Scans the current assembly's directory for plugin assemblies matching <c>Verify.*.dll</c>,
    /// loads each assembly, and invokes its <c>VerifyTests.[AssemblyNameWithPeriodsRemove].Initialize</c>
    /// method if it has not already been initialized.
    /// This allows plugins to register themselves and extend the verification framework.
    /// </summary>
    /// <remarks>
    /// Only plugins that the calling (test) assembly actually references are loaded. This is
    /// determined from the test assembly's <c>.deps.json</c> dependency manifest. When a plugin
    /// reference is removed, .NET does not necessarily remove the plugin assembly from the output
    /// directory, so this prevents those stale assemblies from being loaded (and possibly failing to load).
    /// When no <c>.deps.json</c> is available (for example on .NET Framework) all matching assemblies are loaded.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InitializePlugins()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var testAssembly = Assembly.GetCallingAssembly();
        var directory = Path.GetDirectoryName(GetLocation())!;
        InitializePlugins(directory, testAssembly);
    }

    internal static void InitializePlugins(string directory, Assembly testAssembly)
    {
        // The set of plugins to load is determined from the test assembly's dependency graph
        // (its .deps.json), not from Assembly.GetReferencedAssemblies(). See GetReferencedAssemblyFiles
        // for why. referencedFiles is null when that graph cannot be determined, in which case no
        // filtering is applied and every matching assembly is loaded (the original behavior).
        var referencedFiles = GetReferencedAssemblyFiles(testAssembly);
        foreach (var file in Directory.EnumerateFiles(directory, "Verify.*.dll"))
        {
            if (referencedFiles != null &&
                !referencedFiles.Contains(Path.GetFileName(file)))
            {
                // A Verify.*.dll that is not in the test assembly's dependency graph.
                // This is typically a stale assembly left in the output directory
                // after its package/project reference was removed. Skip it, rather than
                // loading it (or attempting to and failing).
                continue;
            }

            ProcessFile(file);
        }
    }

    /// <summary>
    /// Reads the set of assembly file names (e.g. <c>Verify.Foo.dll</c>) that the <paramref name="testAssembly" />
    /// depends on, based on its <c>.deps.json</c> manifest.
    /// Returns <c>null</c> when no manifest exists, in which case no filtering should be applied. A manifest that
    /// does exist but cannot be read or parsed, or that lists no runtime assemblies, throws (see <see cref="ReadDepsFile" />).
    /// </summary>
    /// <remarks>
    /// The dependency manifest (<c>.deps.json</c>) is used in preference to <see cref="Assembly.GetReferencedAssemblies" />
    /// because they answer different questions:
    /// <list type="bullet">
    /// <item>
    /// <c>GetReferencedAssemblies()</c> reflects the compiled metadata reference table. The C# compiler omits a reference
    /// when no type from that assembly is used in IL. Plugins loaded via this convention are referenced (as a package/project
    /// reference) but their types are typically not used directly in test code (the plugin auto-registers via its
    /// <c>Initialize</c> method). Such a reference is stripped and would be absent from <c>GetReferencedAssemblies()</c>,
    /// which would wrongly exclude a genuinely referenced plugin.
    /// </item>
    /// <item>
    /// <c>.deps.json</c> lists the full (transitive) package/project dependency graph regardless of whether any type is used,
    /// so a real plugin reference is always present, while a stale assembly left in the output directory after its reference
    /// was removed is not. That is exactly the distinction needed here.
    /// </item>
    /// </list>
    /// </remarks>
    internal static HashSet<string>? GetReferencedAssemblyFiles(Assembly testAssembly)
    {
        var location = testAssembly.Location;
        if (location.Length == 0)
        {
            // Dynamic or single-file assembly: no deps.json on disk to resolve.
            return null;
        }

        var directory = Path.GetDirectoryName(location)!;
        var depsFile = Path.Combine(directory, $"{testAssembly.GetName().Name}.deps.json");
        if (!File.Exists(depsFile))
        {
            // No deps.json (for example on .NET Framework). Fall back to loading all plugins.
            // GetReferencedAssemblies() is deliberately not used as a fallback here: it would still
            // strip convention-only plugins (see remarks) and so could exclude a genuinely referenced
            // plugin. A filter that can drop a real plugin is worse than no filter, so load everything.
            return null;
        }

        return ReadDepsFile(depsFile);
    }

    /// <summary>
    /// Reads and validates the runtime assembly file names from the deps.json file at <paramref name="depsFile" />,
    /// which is expected to exist.
    /// </summary>
    /// <remarks>
    /// A deps.json is generated by the build, so a read or parse failure — or a result with no runtime assemblies
    /// (a valid manifest always lists at least the owning assembly's own runtime entry) — indicates a real problem
    /// with the file. It is thrown, naming the file, rather than swallowed and silently degraded to loading all plugins.
    /// </remarks>
    internal static HashSet<string> ReadDepsFile(string depsFile)
    {
        HashSet<string> referenced;
        try
        {
            referenced = ReadReferencedFileNames(File.ReadAllText(depsFile));
        }
        catch (Exception exception)
        {
            throw new($"Failed to read plugin references from deps file '{depsFile}'.", exception);
        }

        if (referenced.Count == 0)
        {
            throw new($"No runtime assemblies were found in deps file '{depsFile}'.");
        }

        return referenced;
    }

    internal static HashSet<string> ReadReferencedFileNames(string depsJsonContent)
    {
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var root = JObject.Parse(depsJsonContent);
        if (root["targets"] is not JObject targets)
        {
            return files;
        }

        foreach (var target in targets.Properties())
        {
            if (target.Value is not JObject libraries)
            {
                continue;
            }

            foreach (var library in libraries.Properties())
            {
                if (library.Value is not JObject libraryValue ||
                    libraryValue["runtime"] is not JObject runtime)
                {
                    continue;
                }

                foreach (var runtimeFile in runtime.Properties())
                {
                    // Runtime keys are like "Verify.Foo.dll" (project refs) or
                    // "lib/net8.0/Verify.Foo.dll" (package refs), so take the file name.
                    files.Add(Path.GetFileName(runtimeFile.Name));
                }
            }
        }

        return files;
    }

    static string GetLocation()
    {
        var assembly = typeof(VerifierSettings).Assembly;
        // ReSharper disable once RedundantSuppressNullableWarningExpression
#pragma warning disable SYSLIB0012
        return new Uri(assembly.CodeBase!).LocalPath;
#pragma warning restore SYSLIB0012
    }

    static void ProcessFile(string file)
    {
        if (!TryGetType(file, out var type))
        {
            return;
        }

        if (GetInitialized(type))
        {
            return;
        }

        InvokeInitialize(type);
    }

    internal static bool TryGetType(string file, [NotNullWhen(true)] out Type? type)
    {
        var assemblyName = Path.GetFileNameWithoutExtension(file);
        if (!assemblyName.StartsWith("Verify."))
        {
            type = null;
            return false;
        }
#pragma warning disable CS0618
        var assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore CS0618
        if (assembly == null)
        {
            throw new($"Could not load assembly '{assemblyName}'.");
        }

        var typeName = GetTypeName(assemblyName);
        type = assembly.GetType(typeName);
        return type != null;
    }

    internal static string GetTypeName(string assemblyName) =>
        $"VerifyTests.{assemblyName.Replace(".", "")}";

    internal static void InvokeInitialize(Type type)
    {
        var method = type
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(_ => _.Name == "Initialize" &&
                        _
                            .GetParameters()
                            .All(_ => _.HasDefaultValue))
            .MinBy(_ => _.GetParameters()
                .Length);
        if (method == null)
        {
            throw new($"Expected {type.Name} to have a method `public static void Initialize()`.");
        }

        var parameters = method
            .GetParameters()
            .Select(_ => _.DefaultValue)
            .ToArray();
        method.Invoke(null, parameters);
    }

    // ReSharper disable once UnusedMember.Local
    static object? DefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }

    internal static bool GetInitialized(Type type)
    {
        var property = type.GetProperty("Initialized", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
        if (property == null)
        {
            throw new($"Expected {type.Name} to have a property `public static bool Initialized {{get;}}` that indicates if Initialize() has been called.");
        }

        return (bool) property.GetValue(null)!;
    }
}