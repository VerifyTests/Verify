using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        internal Namer Namer = new Namer();

        public void UniqueForAssemblyConfiguration()
        {
            Namer.UniqueForAssemblyConfiguration = true;
        }

        public void UniqueForRuntime()
        {
            Namer.UniqueForRuntime = true;
        }

        public void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }

        (string receivedPath, string verifiedPath) GetFileNames(string extension, string? suffix = null)
        {
            var filePrefix = GetFilePrefix();
            if (suffix == null)
            {
                var receivedPath = $"{filePrefix}.received{extension}";
                var verifiedPath = $"{filePrefix}.verified{extension}";
                return (receivedPath, verifiedPath);
            }
            else
            {
                var receivedPath = $"{filePrefix}.{suffix}.received{extension}";
                var verifiedPath = $"{filePrefix}.{suffix}.verified{extension}";
                return (receivedPath, verifiedPath);
            }
        }

        string GetFilePrefix()
        {
            var builder = new StringBuilder(Path.Combine(SourceDirectory, Context.UniqueTestName));

            if (Namer.UniqueForRuntimeAndVersion || Global.Namer.UniqueForRuntimeAndVersion)
            {
                builder.Append($".{Namer.runtimeAndVersion}");
            }
            else if (Namer.UniqueForRuntime || Global.Namer.UniqueForRuntime)
            {
                builder.Append($".{Namer.runtime}");
            }

            if (Namer.UniqueForAssemblyConfiguration || Global.Namer.UniqueForAssemblyConfiguration)
            {
                var assemblyConfiguration = GetAssemblyConfiguration();
                builder.Append($".{assemblyConfiguration}");
            }

            return builder.ToString();
        }

        string GetAssemblyConfiguration()
        {
            var declaringTypeAssembly = Context.MethodInfo.DeclaringType.Assembly;
            var customAttributes = declaringTypeAssembly.GetCustomAttributes();
            var attribute = declaringTypeAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            if (attribute != null)
            {
                return attribute.Configuration;
            }

            throw new Exception("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
        }
    }
}