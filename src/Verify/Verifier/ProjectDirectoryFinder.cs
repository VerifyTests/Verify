static class ProjectDirectoryFinder
{
    public static string Find(string testDirectory)
    {
        if (!TryFind(testDirectory, out var projectDirectory))
        {
            throw new("Could not find project directory");
        }

        return projectDirectory;
    }

    public static bool TryFind(string testDirectory, [NotNullWhen(true)] out string? path)
    {
        var currentDirectory = testDirectory;
        do
        {
            if (Directory.GetFiles(currentDirectory, "*.csproj").Any())
            {
                path = currentDirectory;
                return true;
            }

            var parent = Directory.GetParent(currentDirectory);
            if (parent == null)
            {
                path = null;
                return false;
            }

            currentDirectory = parent.FullName;
        } while (true);
    }
}