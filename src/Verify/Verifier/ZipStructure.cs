static class ZipStructure
{
    public static StringBuilder Build(ZipArchive archive)
    {
        var root = ToNodes(archive);
        var builder = new StringBuilder();
        PrintDirectory(root, 0, builder);
        return builder;
    }

    static DirectoryNode ToNodes(ZipArchive archive)
    {
        var root = new DirectoryNode("");

        foreach (var entry in archive.Entries)
        {
            var parts = entry.FullName.Split('/');
            var current = root;

            foreach (var part in parts)
            {
                if (!current.Children.TryGetValue(part, out var next))
                {
                    next = new(part);
                    current.Children[part] = next;
                }

                current = next;
            }
        }

        return root;
    }

    static void PrintDirectory(DirectoryNode node, int level, StringBuilder builder)
    {
        if (level > 0)
        {
            builder.AppendLine(new string(' ', (level - 1) * 2) + "* " + node.Name);
        }

        foreach (var child in node.Children.Values)
        {
            PrintDirectory(child, level + 1, builder);
        }
    }

    class DirectoryNode(string name)
    {
        public string Name { get; } = name;
        public Dictionary<string, DirectoryNode> Children { get; } = [];
    }
}