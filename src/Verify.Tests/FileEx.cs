public static class FileEx
{
    public static bool IsFileLocked(string file) =>
        IsFileLocked(file, FileAccess.ReadWrite);

    public static bool IsFileReadLocked(string file) =>
        IsFileLocked(file, FileAccess.Read);

    static bool IsFileLocked(string file, FileAccess access)
    {
        try
        {
            using var stream = new FileStream(file, FileMode.Open, access);
            stream.Close();
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }

    public static string GetProjectDirectory([CallerFilePath] string file = "") =>
        FindDirectory(file, "*.csproj");

    public static string GetSolutionDirectory([CallerFilePath] string file = "") =>
        FindDirectory(file, "*.sln");

    public static string GetFileDirectory([CallerFilePath] string file = "") =>
        new FileInfo(file).Directory!.FullName;

    static string FindDirectory(string file, string extension)
    {
        var currentDirectory = new FileInfo(file).Directory!.FullName;
        do
        {
            if (Directory.GetFiles(currentDirectory, extension).Any())
            {
                return currentDirectory;
            }

            var parent = Directory.GetParent(currentDirectory);
            if (parent == null)
            {
                break;
            }

            currentDirectory = parent.FullName;
        } while (true);

        throw new();
    }
}