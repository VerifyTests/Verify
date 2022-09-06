partial class InnerVerifier :
    IDisposable
{
    VerifySettings settings;
    string directory;
    GetFileNames getFileNames = null!;
    GetIndexedFileNames getIndexedFileNames = null!;
    List<string> verifiedFiles = null!;
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

        var uniquenessVerified = GetUniquenessVerified(sharedUniqueness, namer);

        if (settings.useUniqueDirectory)
        {
            InitForDirectoryConvention(uniquenessVerified, typeAndMethod, parameterText);
        }
        else
        {
            InitForFileConvention(sharedUniqueness, namer, uniquenessVerified, typeAndMethod, parameterText);
        }
    }

    void InitForDirectoryConvention(string uniquenessVerified, string typeAndMethod, string parameterText)
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

        var subDirectory = Path.Combine(directory, verifiedPrefix);

        IoHelpers.CreateDirectory(subDirectory);

        verifiedFiles = Directory.EnumerateFiles(subDirectory, "*.verified.*").ToList();

        var prefix = Path.Combine(subDirectory, "target");
        getFileNames = target =>
        {
            var suffix = GetSuffix(target);

            return new(target.Extension, prefix+suffix, prefix+suffix);
        };
        getIndexedFileNames = (target, index) =>
        {
            var suffix = GetIndexSuffix(target, index);

            return new(
                target.Extension,
                $"{prefix}{suffix}",
                $"{prefix}{suffix}");
        };

        IoHelpers.Delete(Directory.EnumerateFiles(subDirectory, "*.received.*"));
    }

    void InitForFileConvention(string sharedUniqueness, Namer namer, string uniquenessVerified, string typeAndMethod, string parameterText)
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

        getFileNames = target =>
        {
            var suffix = GetSuffix(target);

            return new(target.Extension, $"{pathPrefixReceived}{suffix}", $"{pathPrefixVerified}{suffix}");
        };
        getIndexedFileNames = (target, index) =>
        {
            var suffix = GetIndexSuffix(target, index);

            return new(
                target.Extension,
                $"{pathPrefixReceived}{suffix}",
                $"{pathPrefixVerified}{suffix}");
        };

        IoHelpers.Delete(MatchingFileFinder.Find(receivedPrefix, ".received", directory));
    }

    static string GetSuffix(Target target)
    {
        if (target.Name is not null)
        {
            return $"#{target.Name}";
        }

        return "";
    }

    static string GetIndexSuffix(Target target, int index)
    {
        if (target.Name is null)
        {
            return $"#{index:D2}";
        }

        return $"#{target.Name}.{index:D2}";
    }

    static string GetUniquenessVerified(string sharedUniqueness, Namer namer)
    {
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

        return uniquenessVerified;
    }

    static string ResolveDirectory(string sourceFile, VerifySettings settings, PathInfo pathInfo)
    {
        var settingsOrInfoDirectory = settings.Directory ?? pathInfo.Directory;
        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        if (settingsOrInfoDirectory is null)
        {
            return sourceFileDirectory;
        }

        var directory = Path.Combine(sourceFileDirectory, settingsOrInfoDirectory);
        IoHelpers.CreateDirectory(directory);
        return directory;
    }

    public Task<VerifyResult> Verify(object? target, IEnumerable<Target> rawTargets) =>
        VerifyInner(target, null, rawTargets);

    public Task<VerifyResult> Verify(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets);

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