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
        var assembly = Assembly.LoadWithPartialName(assemblyName)!;
#pragma warning restore CS0618
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