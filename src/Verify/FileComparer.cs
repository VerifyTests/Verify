using System;
using System.IO;

static class FileComparer
{
    public static CompareResult DoCompare(string receivedPath, string verifiedPath)
    {
        if (!File.Exists(verifiedPath))
        {
            return CompareResult.MissingVerified;
        }

        if (EmptyFiles.IsEmptyFile(verifiedPath))
        {
            return CompareResult.NotEqual;
        }

        if (!FilesEqual(receivedPath, verifiedPath))
        {
            return CompareResult.NotEqual;
        }

        return CompareResult.Equal;
    }

    public static bool FilesEqual(string path1, string path2)
    {
        return FilesAreEqual(new FileInfo(path1), new FileInfo(path2));
    }

    const int bytesToRead = sizeof(long);

    static bool FilesAreEqual(FileInfo first, FileInfo second)
    {
        if (first.Length != second.Length)
        {
            return false;
        }

        if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var iterations = (int) Math.Ceiling((double) first.Length / bytesToRead);

        using var fs1 = FileHelpers.OpenRead(first.FullName);
        using var fs2 = FileHelpers.OpenRead(second.FullName);
        var one = new byte[bytesToRead];
        var two = new byte[bytesToRead];

        for (var i = 0; i < iterations; i++)
        {
            fs1.Read(one, 0, bytesToRead);
            fs2.Read(two, 0, bytesToRead);

            if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
            {
                return false;
            }
        }

        return true;
    }
}