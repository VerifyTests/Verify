partial class InnerVerifier :
    IDisposable
{
    VerifySettings settings;
    string directory;
    internal GetFileNames GetFileNames { get; }
    internal GetIndexedFileNames GetIndexedFileNames { get; }
    internal List<string> VerifiedFiles { get; }
    internal List<string> ReceivedFiles { get; }
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

        var filesReceived = Directory.EnumerateFiles(directory, $"{receivedFileNamePrefix}.*.*").ToList();
        var filesVerified = Directory.EnumerateFiles(directory, $"{verifiedFileNamePrefix}.*.*").ToList();

        ReceivedFiles = MatchingFileFinder.Find(filesReceived, receivedFileNamePrefix, ".received").ToList();
        VerifiedFiles = MatchingFileFinder.Find(filesVerified, verifiedFileNamePrefix, ".verified").ToList();

        GetFileNames = extension => new(extension, filePathPrefixReceived, filePathPrefixVerified);
        GetIndexedFileNames = (extension, index) => new(extension, $"{filePathPrefixReceived}.{index:D2}", $"{filePathPrefixVerified}.{index:D2}");

        foreach (var file in ReceivedFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }

        VerifierSettings.RunBeforeCallbacks();
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