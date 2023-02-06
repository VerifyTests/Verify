namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void InitializePlugins()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var directory = Path.GetDirectoryName(GetLocation())!;
        foreach (var file in Directory.EnumerateFiles(directory, "Verify.*.dll"))
        {
            ProcessFile(file);
        }
    }

    static string GetLocation()
    {
        var assembly = typeof(VerifierSettings).Assembly;
#if NET5_0_OR_GREATER
        return typeof(VerifierSettings).Assembly.Location;
#else
        return assembly.CodeBase!.Replace("file:///","");
#endif
    }

    static void ProcessFile(string file)
    {
        if (!TryGetType(file, out var type))
        {
            Console.WriteLine($"Did not find type in {file}");
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
        var name = Path.GetFileNameWithoutExtension(file);
        if (!name.StartsWith("Verify."))
        {
            type = null;
            return false;
        }
#pragma warning disable CS0618
        var assembly = Assembly.LoadWithPartialName(name)!;
#pragma warning restore CS0618
        var typeName = name.Replace("Verify.", "VerifyTests.Verify");
        type = assembly.GetType(typeName);
        return type != null;
    }

    internal static void InvokeInitialize(Type type)
    {
        var method = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
        if (method == null)
        {
            throw new($"Expected {type.Name} to have a method `public static void Initialize()`.");
        }

        method.Invoke(null, null);
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