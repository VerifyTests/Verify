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
        VerifierSettings.RunBeforeCallbacks();
        this.settings = settings;

        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, typeName, methodName);
        var typeAndMethod = FileNameBuilder.GetTypeAndMethod(methodName, typeName, settings, pathInfo);
        var parameterText = FileNameBuilder.GetParameterText(methodParameters, settings);

        var namer = settings.Namer;

        var sharedUniqueness = PrefixUnique.SharedUniqueness(namer);

        directory = ResolveDirectory(sourceFile, settings, pathInfo);

        IoHelpers.CreateDirectory(directory);

        var uniquenessVerified = sharedUniqueness;
        if (namer.ResolveUniqueForRuntimeAndVersion())
        {
            uniquenessVerified += $".{Namer.RuntimeAndVersion}";
        }
        else
        {
            if (namer.ResolveUniqueForRuntime())
            {
                uniquenessVerified += $".{Namer.Runtime}";
            }
        }

        if (settings.useUniqueDirectory)
        {
            string verifiedPrefix;
            if (settings.fileName is not null)
            {
                verifiedPrefix = settings.fileName + uniquenessVerified;
            }
            else if (settings.ignoreParametersForVerified)
            {
                verifiedPrefix = $"{typeAndMethod}{uniquenessVerified}";
            }
            else
            {
                verifiedPrefix = $"{typeAndMethod}{parameterText}{uniquenessVerified}";
            }

            var subDirectory = Path.Combine(directory,verifiedPrefix);

            IoHelpers.CreateDirectory(subDirectory);

            verifiedFiles = Directory.EnumerateFiles(subDirectory, "*.verified.*").ToList();

            var prefix = Path.Combine(subDirectory, "target");
            getFileNames = target => new(target.Extension, prefix, prefix);
            getIndexedFileNames = (target, index) =>
            {
                var suffix = GetIndexedSuffix(target, index);
                return new(
                    target.Extension,
                    $"{prefix}.{suffix}",
                    $"{prefix}.{suffix}");
            };

            //DeleteReceivedFiles(receivedPrefix, directory);
        }
        else
        {
            var uniquenessReceived = sharedUniqueness;
            if (namer.ResolveUniqueForRuntimeAndVersion() ||
                TargetAssembly.TargetsMultipleFramework)
            {
                uniquenessReceived += $".{Namer.RuntimeAndVersion}";
            }

            string receivedPrefix;
            string verifiedPrefix;
            if (settings.fileName is not null)
            {
                receivedPrefix = settings.fileName + uniquenessReceived;
                verifiedPrefix = settings.fileName + uniquenessVerified;
            }
            else if (settings.ignoreParametersForVerified)
            {
                receivedPrefix = $"{typeAndMethod}{parameterText}{uniquenessReceived}";
                verifiedPrefix = $"{typeAndMethod}{uniquenessVerified}";
            }
            else
            {
                receivedPrefix = $"{typeAndMethod}{parameterText}{uniquenessReceived}";
                verifiedPrefix = $"{typeAndMethod}{parameterText}{uniquenessVerified}";
            }


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
        }
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

        return Path.Combine(sourceFileDirectory, settingOrPathInfoDirectory);
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