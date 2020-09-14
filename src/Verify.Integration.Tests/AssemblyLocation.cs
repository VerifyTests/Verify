using System.IO;

static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;

        CurrentDirectory = Path.GetDirectoryName(assembly.Location)!;
    }

    public static string CurrentDirectory;
}