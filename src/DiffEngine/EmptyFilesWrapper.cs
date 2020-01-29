using System.IO;
using DiffEngine;

static class EmptyFilesWrapper
{
    public static bool TryWriteEmptyFile(string extension, string path)
    {
        if (Extensions.IsTextExtension(extension))
        {
            File.CreateText(path).Dispose();
            return true;
        }

        if (!EmptyFiles.TryGetPathFor(extension, out var emptyFile))
        {
            return false;
        }

        File.Copy(emptyFile, path, true);
        return true;
    }
}