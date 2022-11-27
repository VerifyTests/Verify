namespace VerifyTests;

public partial class InnerVerifier :
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

    void InitForDirectoryConvention(string uniquenessVerified, string typeAndMethod, string parameters)
    {
        string verifiedPrefix;
        if (settings.fileName is not null)
        {
            verifiedPrefix = $"{settings.fileName}{uniquenessVerified}";
        }
        else if (settings.ignoreParametersForVerified)
        {
            verifiedPrefix = $"{typeAndMethod}{uniquenessVerified}";
        }
        else
        {
            verifiedPrefix = $"{typeAndMethod}{parameters}{uniquenessVerified}";
        }

        if (VerifierSettings.UseUniqueDirectorySplitMode)
        {
            var directoryPrefix = Path.Combine(directory, verifiedPrefix);
            var verifiedDirectory = $"{directoryPrefix}.verified";
            var receivedDirectory = $"{directoryPrefix}.received";
            IoHelpers.CreateDirectory(verifiedDirectory);
            IoHelpers.RenameFiles(
                verifiedDirectory,
                "nofilename.*",
                filePath => filePath.RemoveLast("nofilename"),
                IoHelpers.RenameConflictResolution.Delete);
            verifiedFiles = IoHelpers.Files(verifiedDirectory, "*");

            getFileNames = target =>
            {
                var fileName = $"{target.NameOrTarget}.{target.Extension}";
                return new(
                    target.Extension,
                    Path.Combine(receivedDirectory, fileName),
                    Path.Combine(verifiedDirectory, fileName));
            };
            getIndexedFileNames = (target, index) =>
            {
                var fileName = $"{target.NameOrTarget}#{index}.{target.Extension}";
                return new(
                    target.Extension,
                    Path.Combine(receivedDirectory, fileName),
                    Path.Combine(verifiedDirectory, fileName));
            };

            IoHelpers.DeleteDirectory(receivedDirectory);
        }
        else
        {
            var subDirectory = Path.Combine(directory, verifiedPrefix);
            IoHelpers.CreateDirectory(subDirectory);
            IoHelpers.RenameFiles(
                subDirectory,
                "nofilename.verified.*",
                filePath => filePath.RemoveLast("nofilename"),
                IoHelpers.RenameConflictResolution.Delete);
            verifiedFiles = IoHelpers.Files(subDirectory, "*.verified.*");

            getFileNames = target =>
                new(
                    target.Extension,
                    Path.Combine(subDirectory, $"{target.NameOrTarget}.received.{target.Extension}"),
                    Path.Combine(subDirectory, $"{target.NameOrTarget}.verified.{target.Extension}"));
            getIndexedFileNames = (target, index) =>
                new(
                    target.Extension,
                    Path.Combine(subDirectory, $"{target.NameOrTarget}#{index}.received.{target.Extension}"),
                    Path.Combine(subDirectory, $"{target.NameOrTarget}#{index}.verified.{target.Extension}"));

            IoHelpers.DeleteFiles(subDirectory, "*.received.*");
        }
    }

    void InitForFileConvention(string sharedUniqueness, Namer namer, string uniquenessVerified, string typeAndMethod, string parameters)
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
            receivedPrefix = $"{typeAndMethod}{parameters}{uniquenessReceived}";
            verifiedPrefix = $"{typeAndMethod}{uniquenessVerified}";
        }
        else
        {
            receivedPrefix = $"{typeAndMethod}{parameters}{uniquenessReceived}";
            verifiedPrefix = $"{typeAndMethod}{parameters}{uniquenessVerified}";
        }

        var pathPrefixReceived = Path.Combine(directory, receivedPrefix);
        var pathPrefixVerified = Path.Combine(directory, verifiedPrefix);
        // intentionally do not validate filePathPrefixVerified
        ValidatePrefix(settings, pathPrefixReceived);

        verifiedFiles = MatchingFileFinder.Find(verifiedPrefix, ".verified", directory).ToList();

        getFileNames = target =>
        {
            var suffix = target.Name is null ? "" : $"#{target.Name}";
            return new(
                target.Extension,
                $"{pathPrefixReceived}{suffix}.received.{target.Extension}",
                $"{pathPrefixVerified}{suffix}.verified.{target.Extension}");
        };
        getIndexedFileNames = (target, index) =>
        {
            var suffix = target.Name is null ? $"#{index}" : $"#{target.Name}.{index}";
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
        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        var pathInfoDirectory = pathInfo.Directory;
        if (ContinuousTestingDetector.IsNCrunch)
        {
            var ncrunchProjectDirectory = ContinuousTestingDetector.NCrunchOriginalProjectDirectory;
            var projectDirectory = ProjectDirectoryFinder.Find(sourceFileDirectory);
            sourceFileDirectory = sourceFileDirectory.Replace(projectDirectory, ncrunchProjectDirectory);
            pathInfoDirectory = pathInfoDirectory?.Replace(projectDirectory, ncrunchProjectDirectory);
        }

        var settingsOrInfoDirectory = settings.Directory ?? pathInfoDirectory;

        if (settingsOrInfoDirectory is null)
        {
            return sourceFileDirectory;
        }

        var directory = Path.Combine(sourceFileDirectory, settingsOrInfoDirectory);
        IoHelpers.CreateDirectory(directory);
        return directory;
    }

    public Task<VerifyResult> Verify(object? target, IEnumerable<Target> rawTargets) =>
        VerifyInner(target, null, rawTargets, true);

    public Task<VerifyResult> Verify(IEnumerable<Target> targets) =>
        VerifyInner(targets);

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