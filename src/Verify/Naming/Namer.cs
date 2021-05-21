﻿using System;
using System.Runtime.InteropServices;

namespace VerifyTests
{
    public class Namer
    {
        internal bool UniqueForRuntime;
        internal bool UniqueForAssemblyConfiguration;
        internal bool UniqueForRuntimeAndVersion;
        internal bool UniqueForArchitecture;

        static Namer()
        {
            var (runtime, version) = GetRuntimeAndVersion();
            Runtime = runtime;
            RuntimeAndVersion = $"{runtime}{version.Major}_{version.Minor}";
            Architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        }

        public static string Runtime { get; }

        public static string RuntimeAndVersion { get; }

        public static string Architecture { get; }

        internal Namer()
        {
        }

        internal Namer(Namer namer)
        {
            UniqueForRuntime = namer.UniqueForRuntime;
            UniqueForAssemblyConfiguration = namer.UniqueForAssemblyConfiguration;
            UniqueForRuntimeAndVersion = namer.UniqueForRuntimeAndVersion;
            UniqueForArchitecture = namer.UniqueForArchitecture;
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