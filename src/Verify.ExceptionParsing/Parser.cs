namespace VerifyTests.ExceptionParsing;

public static class Parser
{
    public static Result Parse(IEnumerable<string> lines)
    {
        string? directory = null;
        var delete = new List<string>();
        var notEqual = new List<FilePair>();
        var @new = new List<FilePair>();
        var equal = new List<FilePair>();
        Action<string, IEnumerator<string>>? lineHandler = null;
        using (var enumerator = lines.GetEnumerator())
        {
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

                if (line.StartsWith("Directory:"))
                {
                    directory = line.Substring(11);
                    lineHandler = null;
                    continue;
                }

                if (line.StartsWith("New:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(next, scopedEnum, @new);
                    continue;
                }

                if (line.StartsWith("NotEqual:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(next, scopedEnum, notEqual);
                    continue;
                }

                if (line.StartsWith("Equal:"))
                {
                    lineHandler = (next, scopedEnum) => AddFilePair(next, scopedEnum, equal);
                    continue;
                }

                if (line.StartsWith("Delete:"))
                {
                    lineHandler = (next, _) =>
                    {
                        var trimmed = TrimStart(next, "  - ");
                        delete.Add(trimmed);
                    };
                    continue;
                }

                if (lineHandler != null)
                {
                    lineHandler.Invoke(line, enumerator);
                }
            }
        }

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new("'Directory:' not found");
        }

        return new Result(directory!, @new, notEqual, delete, equal);
    }

    static string TrimStart(string next, string prefix)
    {
        if (!next.StartsWith(prefix))
        {
            throw new($"Expected line to start with `{prefix}`. Line: {next}");
        }

        var trimmed = next.Substring(prefix.Length);

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new($"Expected line to have content after prefix `{prefix}` is trimmed . Line: {next}");
        }

        return trimmed;
    }

    static void AddFilePair(string line, IEnumerator<string> scopedEnumerator, List<FilePair> filePairs)
    {
        var received = TrimStart(line,"  - Received: ");
        var verified = TrimStart(scopedEnumerator.SafeMoveNext(),"    Verified: ");
        filePairs.Add(new FilePair(received, verified));
    }

    static string SafeMoveNext(this IEnumerator<string> enumerator)
    {
        if (!enumerator.MoveNext())
        {
            throw new Exception("Expected more lines");
        }
        return enumerator.Current!;
    }
}