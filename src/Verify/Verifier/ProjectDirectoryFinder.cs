static class ProjectDirectoryFinder
{
    public static string Find(string testDirectory)
    {
        var current = testDirectory;
        do
        {
            if (ContainsProject(current))
            {
                return current;
            }

            var parent = Path.GetDirectoryName(current);

            current = parent ?? throw new("Could not find project directory");
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