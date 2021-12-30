public static class FileEx
{
    public static bool IsFileLocked(string file)
    {
        return IsFileLocked(file, FileAccess.ReadWrite);
    }

    public static bool IsFileReadLocked(string file)
    {
        return IsFileLocked(file, FileAccess.Read);
    }

    static bool IsFileLocked(string file, FileAccess access)
    {
        try
        {
            using var stream = new FileStream(file, FileMode.Open, access);
            stream.Close();
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }
}