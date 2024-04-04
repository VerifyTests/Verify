static class SolutionDirectoryFinder
{
    public static string Find(string testDirectory)
    {
        if (!TryFind(testDirectory, out var solutionDirectory))
        {
            throw new("Could not find solution directory");
        }

        return solutionDirectory;
    }

    public static bool TryFind(string testDirectory, [NotNullWhen(true)] out string? path)
    {
        var currentDirectory = testDirectory;
        do
        {
            if (Directory
                    .GetFiles(currentDirectory, "*.sln")
                    .Length != 0)
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