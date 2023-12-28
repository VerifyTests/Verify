using StackTrace = System.Diagnostics.StackTrace;

namespace VerifyTests;

public partial class InnerVerifier :
    IDisposable
{
    VerifySettings settings;
    string directory;
    GetFileNames getFileNames = null!;
    GetIndexedFileNames getIndexedFileNames = null!;
    IEnumerable<string> verifiedFiles = null!;
    Counter counter;
    internal static bool verifyHasBeenRun;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIfVerifyHasBeenRun()
    {
        if (!verifyHasBeenRun)
        {
            return;
        }

        var stackTrace = new StackTrace(1, false);
        var method = stackTrace.GetFrame(1)!.GetMethod()!;
        var type = method.DeclaringType;
        throw new($"The API '{type}.{method.Name}' must be called prior to any Verify has run. Usually this is done in a [ModuleInitializer].");
    }

    public InnerVerifier(
        string sourceFile,
        VerifySettings settings,
        string typeName,
        string methodName,
        IReadOnlyList<string>? methodParameters,
        PathInfo pathInfo)
    {
        Guard.AgainstEmpty(sourceFile);
        Guard.AgainstEmpty(typeName);
        Guard.AgainstEmpty(methodName);
        verifyHasBeenRun = true;
        VerifierSettings.RunBeforeCallbacks();
        this.settings = settings;

        var typeAndMethod = FileNameBuilder.GetTypeAndMethod(methodName, typeName, settings, pathInfo);
        var parameterText = FileNameBuilder.GetParameterText(methodParameters, settings);

        var namer = settings.Namer;

        directory = ResolveDirectory(sourceFile, settings, pathInfo);

        counter = StartCounter(settings);

        IoHelpers.CreateDirectory(directory);

        if (settings.useUniqueDirectory)
        {
            InitForDirectoryConvention(namer, typeAndMethod, parameterText);
        }
        else
        {
            InitForFileConvention(namer, typeAndMethod, parameterText);
        }
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="InnerVerifier" /> class for verifying the entire file (not just a specific type)
    /// </summary>
    /// <remarks>This constructor is used by 3rd party clients</remarks>
    // ReSharper disable once UnusedMember.Global
    public InnerVerifier(string sourceFile, VerifySettings settings)
    {
        Guard.AgainstEmpty(sourceFile);

        this.settings = settings;
        directory = ResolveDirectory(sourceFile, settings, new());

        counter = StartCounter(settings);

        IoHelpers.CreateDirectory(directory);

        ValidatePrefix(settings, directory);

        var withoutExtension = Path.GetFileNameWithoutExtension(sourceFile);
        verifiedFiles = [Path.Combine(directory, $"{withoutExtension}.verified{Path.GetExtension(sourceFile)}")];

        getFileNames = target =>
            new(
                target.Extension,
                sourceFile,
                Path.Combine(directory, $"{withoutExtension}.verified.{target.Extension}")
            );

        getIndexedFileNames = (_, _) => throw new NotImplementedException();
    }

    static Counter StartCounter(VerifySettings settings) =>
        Counter.Start(
            settings.DateCountingEnable,
#if NET6_0_OR_GREATER
            settings.namedDates,
            settings.namedTimes,
#endif
            settings.namedDateTimes,
            settings.namedGuids,
            settings.namedDateTimeOffsets
        );

    void InitForDirectoryConvention(Namer namer, string typeAndMethod, string parameters)
    {
        var verifiedPrefix = PrefixForDirectoryConvention(namer, typeAndMethod, parameters);

        var directoryPrefix = Path.Combine(directory, verifiedPrefix);

        if (ShouldUseUniqueDirectorySplitMode(settings))
        {
            var verifiedDirectory = $"{directoryPrefix}.verified";
            var receivedDirectory = $"{directoryPrefix}.received";
            IoHelpers.CreateDirectory(verifiedDirectory);
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
            IoHelpers.CreateDirectory(directoryPrefix);
            verifiedFiles = IoHelpers.Files(directoryPrefix, "*.verified.*");

            getFileNames = target =>
                new(
                    target.Extension,
                    Path.Combine(directoryPrefix, $"{target.NameOrTarget}.received.{target.Extension}"),
                    Path.Combine(directoryPrefix, $"{target.NameOrTarget}.verified.{target.Extension}"));
            getIndexedFileNames = (target, index) =>
                new(
                    target.Extension,
                    Path.Combine(directoryPrefix, $"{target.NameOrTarget}#{index}.received.{target.Extension}"),
                    Path.Combine(directoryPrefix, $"{target.NameOrTarget}#{index}.verified.{target.Extension}"));

            IoHelpers.DeleteFiles(directoryPrefix, "*.received.*");
        }
    }

    string PrefixForDirectoryConvention(Namer namer, string typeAndMethod, string parameters)
    {
        var uniquenessVerified = GetUniquenessVerified(PrefixUnique.SharedUniqueness(namer), namer);

        if (settings.fileName is not null)
        {
            return $"{settings.fileName}{uniquenessVerified}";
        }

        if (settings.ignoreParametersForVerified)
        {
            return $"{typeAndMethod}{uniquenessVerified}";
        }

        return $"{typeAndMethod}{parameters}{uniquenessVerified}";
    }

    static bool ShouldUseUniqueDirectorySplitMode(VerifySettings settings) =>
        settings.UseUniqueDirectorySplitMode
            .GetValueOrDefault(VerifierSettings.UseUniqueDirectorySplitMode);

    void InitForFileConvention(Namer namer, string typeAndMethod, string parameters)
    {
        var (receivedPrefix, verifiedPrefix) = PrefixForFileConvention(namer, typeAndMethod, parameters);

        var pathPrefixReceived = Path.Combine(directory, receivedPrefix);
        var pathPrefixVerified = Path.Combine(directory, verifiedPrefix);
        // intentionally do not validate filePathPrefixVerified
        ValidatePrefix(settings, pathPrefixReceived);

        verifiedFiles = MatchingFileFinder.FindVerified(verifiedPrefix, directory);

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

        MatchingFileFinder.DeleteReceived(receivedPrefix, directory);
    }

    (string receivedPrefix, string verifiedPrefix) PrefixForFileConvention(Namer namer, string typeAndMethod, string parameters)
    {
        var sharedUniqueness = PrefixUnique.SharedUniqueness(namer);
        var uniquenessVerified = GetUniquenessVerified(sharedUniqueness, namer);

        if (VerifierSettings.TargetsMultipleFramework)
        {
            sharedUniqueness.Add(Namer.RuntimeAndVersion);
        }

        if (settings.fileName is not null)
        {
            return (
                $"{settings.fileName}{sharedUniqueness}",
                $"{settings.fileName}{uniquenessVerified}");
        }

        if (settings.ignoreParametersForVerified)
        {
            return (
                $"{typeAndMethod}{parameters}{sharedUniqueness}",
                $"{typeAndMethod}{uniquenessVerified}");
        }

        return (
            $"{typeAndMethod}{parameters}{sharedUniqueness}",
            $"{typeAndMethod}{parameters}{uniquenessVerified}");
    }

    static UniquenessList GetUniquenessVerified(UniquenessList sharedUniqueness, Namer namer)
    {
        var uniquenessVerified = new UniquenessList(sharedUniqueness);

        if (namer.ResolveUniqueForRuntimeAndVersion())
        {
            uniquenessVerified.Add(Namer.RuntimeAndVersion);
        }
        else
        {
            if (namer.ResolveUniqueForRuntime())
            {
                uniquenessVerified.Add(Namer.Runtime);
            }
        }

        return uniquenessVerified;
    }

    static string ResolveDirectory(string sourceFile, VerifySettings settings, PathInfo pathInfo)
    {
        var sourceFileDirectory = IoHelpers.ResolveDirectoryFromSourceFile(sourceFile);
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

        var mappedSettingsOrInfoDirectory = IoHelpers.GetMappedBuildPath(settingsOrInfoDirectory);
        var directory = Path.Combine(sourceFileDirectory, mappedSettingsOrInfoDirectory);
        IoHelpers.CreateDirectory(directory);
        return directory;
    }

    public Task<VerifyResult> Verify(object? target, IEnumerable<Target> rawTargets) =>
        VerifyInner(target, null, rawTargets, true);

    public Task<VerifyResult> Verify(Target target) =>
        VerifyInner([target]);

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