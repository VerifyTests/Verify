partial class Verifier
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
        return FileNameBuilder.GetFileNames(extension, suffix, Namer, testType, directory, testName);
    }
}