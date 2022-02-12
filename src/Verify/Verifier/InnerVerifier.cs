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
        var (fileNamePrefix, directory) = fileConvention(uniqueness);

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
        var filePathPrefix = Path.Combine(directory, fileNamePrefix);
        ValidatePrefix(settings, filePathPrefix);

        var pattern = $"{fileNamePrefix}.*.*";
        var files = Directory.EnumerateFiles(directory, pattern).ToList();
        VerifiedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".verified").ToList();
        ReceivedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".received").ToList();

        GetFileNames = extension => new(extension, filePathPrefix);
        GetIndexedFileNames = (extension, index) => new(extension, $"{filePathPrefix}.{index:D2}");

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