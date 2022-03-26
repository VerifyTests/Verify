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
        var (namePrefixReceived, namePrefixVerified, directory) = fileConvention(uniqueness);

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
        var pathPrefixReceived = Path.Combine(directory, namePrefixReceived);
        var pathPrefixVerified = Path.Combine(directory, namePrefixVerified);
        ValidatePrefix(settings, pathPrefixReceived); // intentionally do not validate filePathPrefixVerified

        verifiedFiles = MatchingFileFinder.Find(namePrefixVerified, ".verified", directory).ToList();

        getFileNames = target => new(target.Extension, pathPrefixReceived, pathPrefixVerified);
        getIndexedFileNames = (target, index) =>
        {
            var name = target.Name;
            string suffix;
            if (name == null)
            {
                suffix = $"{index:D2}";
            }
            else
            {
                suffix = $"{index:D2}{name}";
            }
            return new(
                target.Extension,
                $"{pathPrefixReceived}.{suffix}",
                $"{pathPrefixVerified}.{suffix}");
        };

        DeleteReceivedFiles(namePrefixReceived, directory);

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