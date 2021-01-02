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

        internal string? methodName;

        public void UseMethodName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            methodName = name;
        }

        public void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }
    }
}