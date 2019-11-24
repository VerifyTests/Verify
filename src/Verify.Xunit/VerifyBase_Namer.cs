using System.IO;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        internal Namer Namer = new Namer();

        public void UniqueForRuntime()
        {
            Namer.UniqueForRuntime = true;
        }

        public void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }

        (string receivedPath, string verifiedPath) GetFileNames(string extension)
        {
            var filePrefix = GetFilePrefix();
            var receivedPath = $"{filePrefix}.received{extension}";
            var verifiedPath = $"{filePrefix}.verified{extension}";
            return (receivedPath, verifiedPath);
        }

        string GetFilePrefix()
        {
            var filePrefix = Path.Combine(SourceDirectory, Context.UniqueTestName);
            if (Namer.UniqueForRuntimeAndVersion || Global.Namer.UniqueForRuntimeAndVersion)
            {
                return $"{filePrefix}.{Namer.runtimeAndVersion}";
            }
            if (Namer.UniqueForRuntime || Global.Namer.UniqueForRuntime)
            {
                return $"{filePrefix}.{Namer.runtime}";
            }
            return filePrefix;
        }
    }
}