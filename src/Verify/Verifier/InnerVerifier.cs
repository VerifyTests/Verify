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
        List<string> methodParameters,
        PathInfo pathInfo)
    {
        VerifierSettings.RunBeforeCallbacks();
        this.settings = settings;

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


        if (VerifierSettings.UseUniqueDirectorySplitMode)
        {
            var directoryPrefix = Path.Combine(directory, verifiedPrefix);
            var verifiedDirectory = $"{directoryPrefix}.verified";
            var receivedDirectory = $"{directoryPrefix}.received";
            IoHelpers.CreateDirectory(verifiedDirectory);
            IoHelpers.CreateDirectory(receivedDirectory);

            verifiedFiles = IoHelpers.Files(verifiedDirectory, "*");

            getFileNames = target =>
            {
                var name = target.Name ?? "target";
                var verifiedPath = Path.Combine(verifiedDirectory, name);
                var receivedPath = Path.Combine(receivedDirectory, name);
                return new(
                    target.Extension,
                    $"{receivedPath}.received.{target.Extension}",
                    $"{verifiedPath}.verified.{target.Extension}");
            };
            getIndexedFileNames = (target, index) =>
            {
                string verifiedPath;
                string receivedPath;
                if (target.Name is null)
                {
                    var fileName = $"target#{index:D2}";
                    receivedPath = Path.Combine(receivedDirectory, fileName);
                    verifiedPath = Path.Combine(verifiedDirectory, fileName);
                }
                else
                {
                    var fileName = $"{target.Name}#{index:D2}";
                    receivedPath = Path.Combine(receivedDirectory, fileName);
                    verifiedPath = Path.Combine(verifiedDirectory, fileName);
                }

                return new(
                    target.Extension,
                    $"{receivedPath}.received.{target.Extension}",
                    $"{verifiedPath}.verified.{target.Extension}");
            };

            IoHelpers.DeleteFiles(receivedDirectory, "*");
        }
        else
        {
            var subDirectory = Path.Combine(directory, verifiedPrefix);
            IoHelpers.CreateDirectory(subDirectory);
            verifiedFiles = IoHelpers.Files(subDirectory, "*.verified.*");

            getFileNames = target =>
            {
                var path = Path.Combine(subDirectory, target.Name ?? "target");
                return new(
                    target.Extension,
                    $"{path}.received.{target.Extension}",
                    $"{path}.verified.{target.Extension}");
            };
            getIndexedFileNames = (target, index) =>
            {
                string path;
                if (target.Name is null)
                {
                    path = Path.Combine(subDirectory, $"target#{index:D2}");
                }
                else
                {
                    path = Path.Combine(subDirectory, $"{target.Name}#{index:D2}");
                }

                return new(
                    target.Extension,
                    $"{path}.received.{target.Extension}",
                    $"{path}.verified.{target.Extension}");
            };

            IoHelpers.DeleteFiles(subDirectory, "*.received.*");
        }
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
            string suffix;
            if (target.Name is not null)
            {
                suffix = $"#{target.Name}";
            }
            else
            {
                suffix = "";
            }

            return new(target.Extension,
                $"{pathPrefixReceived}{suffix}.received.{target.Extension}",
                $"{pathPrefixVerified}{suffix}.verified.{target.Extension}");
        };
        getIndexedFileNames = (target, index) =>
        {
            string suffix;
            if (target.Name is null)
            {
                suffix = $"#{index:D2}";
            }
            else
            {
                suffix = $"#{target.Name}.{index:D2}";
            }

            return new(
                target.Extension,
                $"{pathPrefixReceived}{suffix}.received.{target.Extension}",
                $"{pathPrefixVerified}{suffix}.verified.{target.Extension}");
        };

        IoHelpers.DeleteFiles(MatchingFileFinder.Find(receivedPrefix, ".received", directory));
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