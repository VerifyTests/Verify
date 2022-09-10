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

        var subDirectory = Path.Combine(directory, verifiedPrefix);

        IoHelpers.CreateDirectory(subDirectory);

        if (VerifierSettings.UseUniqueDirectorySplitMode)
        {
            var verifiedDirectory = Path.Combine(directory, ".verified");
            var receivedDirectory = Path.Combine(directory, ".received");
            verifiedFiles = Directory.EnumerateFiles(verifiedDirectory, "*").ToList();

            getFileNames = target =>
            {
                var verifiedPath = Path.Combine(subDirectory, target.Name ?? "target");
                var receivedPath = Path.Combine(subDirectory, target.Name ?? "target");
                return new(target.Extension, verifiedPath, receivedPath);
            };
            getIndexedFileNames = (target, index) =>
            {
                string verifiedPath;
                string receivedPath;
                if (target.Name is null)
                {
                    verifiedPath = Path.Combine(subDirectory, $"target#{index:D2}");
                    receivedPath = Path.Combine(subDirectory, $"target#{index:D2}");
                }
                else
                {
                    verifiedPath = Path.Combine(subDirectory, $"{target.Name}#{index:D2}");
                    receivedPath = Path.Combine(subDirectory, $"{target.Name}#{index:D2}");
                }

                return new(target.Extension, verifiedPath, path);
            };

            IoHelpers.Delete(Directory.EnumerateFiles(receivedDirectory, "*", SearchOption.AllDirectories));
        }
        else
        {
            verifiedFiles = Directory.EnumerateFiles(subDirectory, "*.verified.*").ToList();

            getFileNames = target =>
            {
                var path = Path.Combine(subDirectory, target.Name ?? "target");
                return new(target.Extension, path, path);
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

                return new(target.Extension, path, path);
            };

            IoHelpers.Delete(Directory.EnumerateFiles(subDirectory, "*.received.*", SearchOption.AllDirectories));
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

            return new(target.Extension, $"{pathPrefixReceived}{suffix}", $"{pathPrefixVerified}{suffix}");
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
                $"{pathPrefixReceived}{suffix}",
                $"{pathPrefixVerified}{suffix}");
        };

        IoHelpers.Delete(MatchingFileFinder.Find(receivedPrefix, ".received", directory));
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