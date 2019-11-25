namespace VerifyXunit
{
    public static partial class Global
    {
        internal static Namer Namer = new Namer();

        public static void UniqueForRuntime()
        {
            Namer.UniqueForRuntime = true;
        }

        public static void UniqueForAssemblyConfiguration()
        {
            Namer.UniqueForAssemblyConfiguration = true;
        }

        public static void UniqueForRuntimeAndVersion()
        {
            Namer.UniqueForRuntimeAndVersion = true;
        }
    }
}