partial class InnerVerifier :
    IDisposable
{
    VerifySettings settings;
    string directory;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;
    List<string> verifiedFiles;
    Counter counter = Counter.Start();

    public InnerVerifier(
        string sourceFile,
        VerifySettings settings,
        string typeName,
        string methodName,
        List<string> methodParameters)
    {
        this.settings = settings;

        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, typeName, methodName);
        var (receivedPrefix, verifiedPrefix) = FileNameBuilder.Build(methodName, typeName, settings, methodParameters, pathInfo);

        directory = ResolveDirectory(sourceFile, settings, pathInfo);

        var pathPrefixReceived = Path.Combine(directory, receivedPrefix);
        var pathPrefixVerified = Path.Combine(directory, verifiedPrefix);
        // intentionally do not validate filePathPrefixVerified
        ValidatePrefix(settings, pathPrefixReceived);

        verifiedFiles = MatchingFileFinder.Find(verifiedPrefix, ".verified", directory).ToList();

        getFileNames = target => new(target.Extension, pathPrefixReceived, pathPrefixVerified);
        getIndexedFileNames = (target, index) =>
        {
            var suffix = GetIndexedSuffix(target, index);
            return new(
                target.Extension,
                $"{pathPrefixReceived}.{suffix}",
                $"{pathPrefixVerified}.{suffix}");
        };

        DeleteReceivedFiles(receivedPrefix, directory);

        VerifierSettings.RunBeforeCallbacks();
    }

    static string GetIndexedSuffix(Target target, int index)
    {
        if (target.Name is null)
        {
            return $"{index:D2}";
        }

        return $"{index:D2}{target.Name}";
    }

    static string ResolveDirectory(string sourceFile, VerifySettings settings, PathInfo pathInfo)
    {
        var settingOrPathInfoDirectory = settings.Directory ?? pathInfo.Directory;
        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        if (settingOrPathInfoDirectory is null)
        {
            return sourceFileDirectory;
        }

        var directory = Path.Combine(sourceFileDirectory, settingOrPathInfoDirectory);
        IoHelpers.CreateDirectory(directory);
        return directory;
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

    public Task<VerifyResult> Verify(object? target, Func<Task>? cleanup, IEnumerable<Target> targets) =>
        VerifyInner(target, cleanup, targets);

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