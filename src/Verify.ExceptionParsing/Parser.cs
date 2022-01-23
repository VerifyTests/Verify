namespace VerifyTests.ExceptionParsing;

public static class Parser
{
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
            if (!enumerator.MoveNext() || !enumerator.Current!.StartsWith("Directory: "))
            {
                throw new ParseException("Expected content to contain `Directory:` at the start.");
            }

            var directory = enumerator.Current.Substring(11);

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ParseException("Empty 'Directory:'");
            }

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

                if (lineHandler != null)
                {
                    lineHandler.Invoke(line, enumerator);
                }
            }
        }

        return new Result(@new, notEqual, delete, equal);
    }

    static string TrimStart(string next, string prefix)
    {
        if (!next.StartsWith(prefix))
        {
            throw new ParseException($"Expected line to start with `{prefix}`. Line: {next}");
        }

        var trimmed = next.Substring(prefix.Length);

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ParseException($"Expected line to have content after prefix `{prefix}` is trimmed . Line: {next}");
        }

        return trimmed;
    }

    static void AddFilePair(string directory, string line, IEnumerator<string> scopedEnumerator, List<FilePair> filePairs)
    {
        var received = TrimStart(line, "  - Received: ");
        var verified = TrimStart(scopedEnumerator.SafeMoveNext(), "    Verified: ");
        filePairs.Add(
            new FilePair(
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