using System.Runtime.InteropServices;

namespace VerifyTests
{
    public class Namer
    {
        static string? assemblyConfig;
        public static string AssemblyConfig
        {
            get
            {
                if (assemblyConfig != null)
                {
                    return assemblyConfig;
                }
                
                throw new("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
            }
        }

        internal static void UseAssembly(Assembly assembly)
        {
            assemblyConfig = assembly.GetAttributeConfiguration();
        }

        internal bool UniqueForRuntime;
        internal bool UniqueForAssemblyConfiguration;
        internal bool UniqueForRuntimeAndVersion;
        internal bool UniqueForArchitecture;
        internal bool UniqueForOSPlatform;

        static Namer()
        {
            var (runtime, version) = GetRuntimeAndVersion();
            Runtime = runtime;
            RuntimeAndVersion = $"{runtime}{version.Major}_{version.Minor}";
            Architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
            OperatingSystemPlatform = GetOSPlatform();
        }

        static string GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
           
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }

            throw new("Unknown OS");
        }

        public static string Runtime { get; }

        public static string RuntimeAndVersion { get; }

        public static string Architecture { get; }

        public static string OperatingSystemPlatform { get; }


        internal Namer()
        {
        }

        internal Namer(Namer namer)
        {
            UniqueForRuntime = namer.UniqueForRuntime;
            UniqueForAssemblyConfiguration = namer.UniqueForAssemblyConfiguration;
            UniqueForRuntimeAndVersion = namer.UniqueForRuntimeAndVersion;
            UniqueForArchitecture = namer.UniqueForArchitecture;
            UniqueForOSPlatform = namer.UniqueForOSPlatform;
        }

        static (string runtime, Version Version) GetRuntimeAndVersion()
        {
            var description = RuntimeInformation.FrameworkDescription;

            if (description.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
            {
                var version = Version.Parse(description.Replace(".NET Framework ", ""));
                return ("Net", version);
            }

            var environmentVersion = Environment.Version;

            if (description.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase))
            {
                return ("Core", environmentVersion);
            }

            if (description.StartsWith(".NET", StringComparison.OrdinalIgnoreCase))
            {
                return ("DotNet", environmentVersion);
            }

            if (description.StartsWith("Mono", StringComparison.OrdinalIgnoreCase))
            {
                return ("Mono", environmentVersion);
            }

            throw new($"Could not resolve runtime for '{description}'.");
        }
    }
}