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

        public void UseName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Namer.Name = name;
        }

        public void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }
    }
}