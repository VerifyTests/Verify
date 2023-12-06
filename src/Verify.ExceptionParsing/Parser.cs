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
        Action<string, IEnumerator<string>>? lineHandler = null;
        using (var enumerator = lines.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                throw new ParseException("No content");
            }

            var firstLine = enumerator.Current!;
            //MsTest exception start with "Test method..." so lets swallow them
            if (firstLine.StartsWith("Test method"))
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

                if (line.StartsWith("FileContent:"))
                {
                    break;
                }

                if (line.StartsWith("New:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, @new);
                    continue;
                }

                if (line.StartsWith("NotEqual:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, notEqual);
                    continue;
                }

                if (line.StartsWith("Equal:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(directory, next, scopedEnum, equal);
                    continue;
                }

                if (line.StartsWith("Delete:"))
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

        return new(@new, notEqual, delete, equal);
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

        if (firstLine.StartsWith("VerifyException : Directory: "))
        {
            var directory = firstLine[29..];

            ThrowIfEmpty(directory);

            return directory;
        }

        // MsTest
        if (firstLine.StartsWith("VerifyException: Directory: "))
        {
            var directory = firstLine[28..];

            ThrowIfEmpty(directory);

            return directory;
        }

        if (firstLine.StartsWith("Directory: "))
        {
            var directory = firstLine[11..];

            ThrowIfEmpty(directory);

            return directory;
        }

        throw new ParseException("Expected content to contain `Directory:`, or `VerifyException : Directory:`, or `VerifyException: Directory:` at the start.");
    }

    static string TrimStart(string next, string prefix)
    {
        if (!next.StartsWith(prefix))
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