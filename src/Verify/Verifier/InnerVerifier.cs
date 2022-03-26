partial class InnerVerifier :
    IDisposable
{
    VerifySettings settings;
    string directory;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;
    List<string> verifiedFiles;
    Counter counter;

    public InnerVerifier(string sourceFile, VerifySettings settings, GetFileConvention fileConvention)
    {
        counter = Counter.Start();
        this.settings = settings;

        var uniqueness = PrefixUnique.GetUniqueness(settings.Namer);
        var (receivedFileNamePrefix, verifiedFileNamePrefix, directory) = fileConvention(uniqueness);

        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        if (directory is null)
        {
            directory = sourceFileDirectory;
        }
        else
        {
            directory = Path.Combine(sourceFileDirectory, directory);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        this.directory = directory;
        var filePathPrefixReceived = Path.Combine(directory, receivedFileNamePrefix);
        var filePathPrefixVerified = Path.Combine(directory, verifiedFileNamePrefix);
        ValidatePrefix(settings, filePathPrefixReceived); // intentionally do not validate filePathPrefixVerified

        verifiedFiles = MatchingFileFinder.Find(verifiedFileNamePrefix, ".verified", directory).ToList();

        getFileNames = extension => new(extension, filePathPrefixReceived, filePathPrefixVerified);
        getIndexedFileNames = (extension, index) => new(extension, $"{filePathPrefixReceived}.{index:D2}", $"{filePathPrefixVerified}.{index:D2}");

        DeleteReceivedFiles(receivedFileNamePrefix, directory);

        VerifierSettings.RunBeforeCallbacks();
    }

    static void DeleteReceivedFiles(string receivedFileNamePrefix, string directory)
    {
        foreach (var file in MatchingFileFinder.Find(receivedFileNamePrefix, ".received", directory))
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }
    }

    static void ValidatePrefix(VerifySettings settings, string filePathPrefix)
    {
        if (settings.UniquePrefixDisabled || VerifierSettings.UniquePrefixDisabled)
        {
            return;
        }

        PrefixUnique.CheckPrefixIsUnique(filePathPrefix);
    }

    public void Dispose()
    {
        VerifierSettings.RunAfterCallbacks();

        Counter.Stop();
    }
}