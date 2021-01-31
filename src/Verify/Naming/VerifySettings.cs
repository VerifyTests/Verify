using System.Runtime.CompilerServices;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal Namer Namer = new();

        public void UniqueForAssemblyConfiguration()
        {
            Namer.UniqueForAssemblyConfiguration = true;
        }

        public void UniqueForRuntime()
        {
            Namer.UniqueForRuntime = true;
        }

        internal string? directory;

        /// <summary>
        /// Use a custom directory for the test results.
        /// </summary>
        public void UseDirectory(string directory)
        {
            Guard.AgainstNullOrEmpty(directory, nameof(directory));
            this.directory = directory;
        }

        internal string? typeName;

        /// <summary>
        /// Use a custom class name for the test results.
        /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}.{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
        /// </summary>
        /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
        public void UseTypeName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            ThrowIfFileNameDefined();

            typeName = name;
        }

        internal string? methodName;

        /// <summary>
        /// Use a custom method name for the test results.
        /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}.{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
        /// </summary>
        /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
        public void UseMethodName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            ThrowIfFileNameDefined();

            methodName = name;
        }

        internal string? fileName;

        /// <summary>
        /// Use a file name for the test results.
        /// Overrides the `{TestClassName}.{TestMethodName}.{Parameters}` parts of the file naming.
        /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}.{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
        /// </summary>
        /// <remarks>Not compatible with <see cref="UseTypeName"/>, <see cref="UseMethodName"/>, or <see cref="UseParameters"/>.</remarks>
        public void UseFileName(string fileName)
        {
            Guard.AgainstNullOrEmpty(fileName, nameof(fileName));
            ThrowIfMethodOrTypeNameDefined();

            this.fileName = fileName;
        }

        void ThrowIfMethodOrTypeNameDefined()
        {
            if (methodName != null ||
                typeName != null)
            {
                throw new($"{nameof(UseFileName)} is not compatible with {nameof(UseMethodName)} or {nameof(UseTypeName)}.");
            }
        }


        void ThrowIfFileNameDefined([CallerMemberName] string caller = "")
        {
            if (fileName != null)
            {
                throw new($"{caller} is not compatible with {nameof(UseFileName)}.");
            }
        }

        public void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }
    }
}