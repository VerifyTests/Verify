static class PathLauncher
{
    public static void Launch(string path)
    {
        if (BuildServerDetector.Detected)
        {
            throw new("OpenExplorerAndDebug is not supported on build servers.");
        }

        using var process = Process.Start(Command(), path);
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
        else
        {
            Debugger.Launch();
        }

        static string Command()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "explorer.exe";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "open";
            }

            throw new($"Unsupported operating system: {RuntimeInformation.OSDescription}");
        }
    }
}