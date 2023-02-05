namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void InitializePlugins()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var directory = Path.GetDirectoryName(typeof(VerifierSettings).Assembly.Location)!;
        foreach (var file in Directory.EnumerateFiles(directory, "Verify.*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (!name.StartsWith("Verify."))
            {
                continue;
            }

            var assembly = Assembly.Load(name);
            var typeName = name.Replace("Verify.", "VerifyTests.Verify");
            var type = assembly.GetType(typeName);
            if (type == null)
            {
                continue;
            }

            var initialized = GetInitialized(type);

            if (!initialized)
            {
                InvokeInitialize(type);
            }
        }
    }

    static void InvokeInitialize(Type type)
    {
        var method = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
        if (method == null)
        {
            throw new($"Expected {type.Name} to have a method `public static void Initialize()`.");
        }

        method.Invoke(null, null);
    }

    static bool GetInitialized(Type type)
    {
        var property = type.GetProperty("Initialized", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
        if (property == null)
        {
            throw new($"Expected {type.Name} to have a property `public static bool Initialized {{get;}}` that indicates if Initialize() has been called.");
        }

        return (bool) property.GetValue(null)!;
    }
}