using System.Runtime.CompilerServices;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal Namer Namer = new();

        public void UniqueForAssemblyConfiguration()
        {
            CheckUseFileName();
            Namer.UniqueForAssemblyConfiguration = true;
        }

        public void UniqueForRuntime()
        {
            CheckUseFileName();
            Namer.UniqueForRuntime = true;
        }

        internal string? directory;

        public void UseDirectory(string directory)
        {
            Guard.AgainstNullOrEmpty(directory, nameof(directory));
            this.directory = directory;
        }

        internal string? typeName;

        public void UseTypeName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            CheckUseFileName();

            typeName = name;
        }

        internal string? methodName;

        public void UseMethodName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
          CheckUseFileName();

            methodName = name;
        }

        internal string? fileName;

        public void UseFileName(string fileName)
        {
            Guard.AgainstNullOrEmpty(fileName, nameof(fileName));
            CheckUseFileName();

            this.fileName = fileName;
        }

        void CheckUseFileName([CallerMemberName] string caller = "")
        {
            if (methodName != null ||
                typeName != null ||
                Namer.UniqueForRuntimeAndVersion ||
                Namer.UniqueForRuntime ||
                Namer.UniqueForAssemblyConfiguration)
            {
                throw new($"{caller} is not compatible with UseMethodName or UseTypeName.");
            }
        }

        public void UniqueForRuntimeAndVersion()
        {
            CheckUseFileName();
            Namer.UniqueForRuntimeAndVersion = true;
        }
    }
}