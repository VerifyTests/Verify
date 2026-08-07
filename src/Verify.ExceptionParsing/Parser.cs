namespace VerifyTests.ExceptionParsing;

public static class Parser
{
    static string[] newlines = ["\r\n", "\r", "\n"];

    public static Result Parse(string message)
    {
        var lines = message.Split(newlines, StringSplitOptions.RemoveEmptyEntries);
        return Parse(lines);
    }

    public static Result Parse(IEnumerable<string> lines)
    {
        try
        {
            return InnerParse(lines);
        }
        catch (ParseException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ParseException("Failed to parse content.", exception);
        }
    }

    static Result InnerParse(IEnumerable<string> lines)
    {
        var delete = new List<string>();
        var notEqual = new List<FilePair>();
        var @new = new List<FilePair>();
        var equal = new List<FilePair>();
        var inlineNew = new List<InlineEntryBuilder>();
        var inlineNotEqual = new List<InlineEntryBuilder>();
        Action<string, IEnumerator<string>>? lineHandler = null;
        using (var enumerator = lines.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                throw new ParseException("No content");
            }

            var firstLine = enumerator.Current!;
            //MsTest exception start with "Test method..." so lets swallow them
            if (firstLine.StartsWith("Test method", StringComparison.Ordinal))
            {
                if (!enumerator.MoveNext())
                {
                    throw new ParseException("No content");
                }

                firstLine = enumerator.Current!;
            }

            var directory = GetDirectory(firstLine);

            while (enumerator.MoveNext())
            {
                var line = enumerator.Current!;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("FileContent:", StringComparison.Ordinal))
                {
                    break;
                }

                // Inline sections are checked before New/NotEqual for
                // specific-before-general ordering
                if (line.StartsWith("InlineNew:", StringComparison.Ordinal))
                {
                    lineHandler = (next, _) => AddInlineLine(next, inlineNew);
                    continue;
                }

                if (line.StartsWith("InlineNotEqual:", StringComparison.Ordinal))
                {
                    lineHandler = (next, _) => AddInlineLine(next, inlineNotEqual);
                    continue;
                }

                if (line.StartsWith("New:", StringComparison.Ordinal))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, @new);
                    continue;
                }

                if (line.StartsWith("NotEqual:", StringComparison.Ordinal))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, notEqual);
                    continue;
                }

                if (line.StartsWith("Equal:", StringComparison.Ordinal))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, equal);
                    continue;
                }

                if (line.StartsWith("Delete:", StringComparison.Ordinal))
                {
                    lineHandler = (next, _) =>
                    {
                        var trimmed = TrimStart(next, "  - ");
                        delete.Add(Path.Combine(directory, trimmed));
                    };
                    continue;
                }

                lineHandler?.Invoke(line, enumerator);
            }
        }

        return new(
            @new,
            notEqual,
            delete,
            equal,
            inlineNew.Select(_ => _.Build()).ToList(),
            inlineNotEqual.Select(_ => _.Build()).ToList());
    }

    class InlineEntryBuilder
    {
        public string SourceFile = "";
        public int Line;
        public string? ReceivedPath;
        public string? ExpectedPath;
        public string? PatchPath;

        public InlineEntry Build() =>
            new(SourceFile, Line, ReceivedPath, ExpectedPath, PatchPath);
    }

    static void AddInlineLine(string line, List<InlineEntryBuilder> entries)
    {
        if (line.StartsWith("  - Source: ", StringComparison.Ordinal))
        {
            var value = TrimStart(line, "  - Source: ");
            // Split on the last colon: Windows drive letters contain one
            var index = value.LastIndexOf(':');
            if (index < 1 ||
                !int.TryParse(value[(index + 1)..], out var lineNumber))
            {
                throw new ParseException($"Expected `path:line` after `Source: `. Line: {line}");
            }

            entries.Add(
                new()
                {
                    SourceFile = value[..index],
                    Line = lineNumber
                });
            return;
        }

        if (entries.Count == 0)
        {
            throw new ParseException($"Expected `  - Source: ` line. Line: {line}");
        }

        var entry = entries[^1];
        if (line.StartsWith("    Received: ", StringComparison.Ordinal))
        {
            entry.ReceivedPath = TrimStart(line, "    Received: ");
            return;
        }

        if (line.StartsWith("    Expected: ", StringComparison.Ordinal))
        {
            entry.ExpectedPath = TrimStart(line, "    Expected: ");
            return;
        }

        if (line.StartsWith("    Patch: ", StringComparison.Ordinal))
        {
            entry.PatchPath = TrimStart(line, "    Patch: ");
            return;
        }

        throw new ParseException($"Unexpected line in inline section. Line: {line}");
    }

    static string GetDirectory(string firstLine)
    {
        static void ThrowIfEmpty(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ParseException("Empty 'Directory'");
            }
        }

        if (firstLine.StartsWith("VerifyException : Directory: ", StringComparison.Ordinal))
        {
            var directory = firstLine[29..];

            ThrowIfEmpty(directory);

            return directory;
        }

        // MsTest
        if (firstLine.StartsWith("VerifyException: Directory: ", StringComparison.Ordinal))
        {
            var directory = firstLine[28..];

            ThrowIfEmpty(directory);

            return directory;
        }

        if (firstLine.StartsWith("Directory: ", StringComparison.Ordinal))
        {
            var directory = firstLine[11..];

            ThrowIfEmpty(directory);

            return directory;
        }

        throw new ParseException("Expected content to contain `Directory:`, or `VerifyException : Directory:`, or `VerifyException: Directory:` at the start.");
    }

    static string TrimStart(string next, string prefix)
    {
        if (!next.StartsWith(prefix, StringComparison.Ordinal))
        {
            throw new ParseException($"Expected line to start with `{prefix}`. Line: {next}");
        }

        var trimmed = next[prefix.Length..];

        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            return trimmed;
        }

        throw new ParseException($"Expected line to have content after prefix `{prefix}` is trimmed . Line: {next}");
    }

    static void AddFilePair(string directory, string line, IEnumerator<string> scopedEnumerator, List<FilePair> filePairs)
    {
        var received = TrimStart(line, "  - Received: ");
        var verified = TrimStart(scopedEnumerator.SafeMoveNext(), "    Verified: ");
        filePairs.Add(
            new(
                Path.Combine(directory, received),
                Path.Combine(directory, verified)));
    }

    static string SafeMoveNext(this IEnumerator<string> enumerator)
    {
        if (!enumerator.MoveNext())
        {
            throw new ParseException("Expected more lines");
        }

        return enumerator.Current!;
    }
}