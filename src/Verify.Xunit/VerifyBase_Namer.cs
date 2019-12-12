namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public void UniqueForAssemblyConfiguration()
        {
            verifier.UniqueForAssemblyConfiguration();
        }

        public void UniqueForRuntime()
        {
            verifier.UniqueForRuntime();
        }

        public void UniqueForRuntimeAndVersion()
        {
            verifier.UniqueForRuntimeAndVersion();
        }
    }
}