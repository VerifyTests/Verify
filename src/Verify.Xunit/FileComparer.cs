using System;
using System.IO;

static class FileComparer
{
    public static VerifyResult DoCompare(string receivedPath, string verifiedPath, string extension)
    {
        if (!File.Exists(verifiedPath))
        {
            return VerifyResult.MissingVerified;
        }

        if (EmptyFiles.IsEmptyFile(extension, verifiedPath))
        {
            return VerifyResult.NotEqual;
        }

        if (!FilesEqual(receivedPath, verifiedPath))
        {
            return VerifyResult.NotEqual;
        }

        return VerifyResult.Equal;
    }
    public static bool FilesEqual(string path1, string path2)
    {
        return FilesAreEqual(new FileInfo(path1), new FileInfo(path2));
    }

    const int BYTES_TO_READ = sizeof(long);

    static bool FilesAreEqual(FileInfo first, FileInfo second)
    {
        if (first.Length != second.Length)
            return false;

        if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            return true;

        var iterations = (int) Math.Ceiling((double) first.Length / BYTES_TO_READ);

        using var fs1 = first.OpenRead();
        using var fs2 = second.OpenRead();
        var one = new byte[BYTES_TO_READ];
        var two = new byte[BYTES_TO_READ];

        for (var i = 0; i < iterations; i++)
        {
            fs1.Read(one, 0, BYTES_TO_READ);
            fs2.Read(two, 0, BYTES_TO_READ);

            if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
            {
                return false;
            }
        }

        return true;
    }

}