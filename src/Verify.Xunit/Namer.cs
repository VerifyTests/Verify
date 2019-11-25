using System;
using System.Runtime.InteropServices;

class Namer
{
    public bool UniqueForRuntime = false;
    public bool UniqueForAssemblyConfiguration = false;
    public bool UniqueForRuntimeAndVersion = false;

    static Namer()
    {
        var (runtime, version) = GetRuntimeAndVersion();
        Namer.runtime = runtime;
        runtimeAndVersion = $"{runtime}{version.Major}_{version.Minor}";
    }

    public static string runtime;

    public static string runtimeAndVersion;

    public static (string runtime, Version Version) GetRuntimeAndVersion()
    {
        var description = RuntimeInformation.FrameworkDescription;
        if (description.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
        {
            var version = Version.Parse(description.Replace(".NET Framework ", ""));
            return ("Net", version);
        }

        if (description.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase))
        {
            return ("Core", Environment.Version);
        }

        throw new Exception($"Could not resolve runtime for '{description}'.");
    }
}