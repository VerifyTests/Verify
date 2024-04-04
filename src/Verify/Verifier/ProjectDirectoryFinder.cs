static class ProjectDirectoryFinder
{
    public static string Find(string testDirectory)
    {
        var currentDirectory = testDirectory;
        do
        {
            if (ContainsProject(currentDirectory))
            {
                return currentDirectory;
            }

            var parent = Path.GetDirectoryName(currentDirectory);
            if (parent == null)
            {
                throw new("Could not find project directory");
            }

            currentDirectory = parent;
        } while (true);
    }

    static bool ContainsProject(string currentDirectory) =>
        ContainsProject(currentDirectory, "*.csproj") ||
        ContainsProject(currentDirectory, "*.fsproj") ||
        ContainsProject(currentDirectory, "*.vbproj");

    static bool ContainsProject(string currentDirectory, string pattern)
    {
        foreach (var unused in Directory.EnumerateFiles(currentDirectory, pattern))
        {
            return true;
        }

        return false;
    }
}