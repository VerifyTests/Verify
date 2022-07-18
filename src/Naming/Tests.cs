using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    MethodInfo methodInfo;
    Type type;
    string sourceFile;

    public Tests()
    {
        var directory = Path.Combine(Path.GetTempPath(), "VerifyNamer");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);
        methodInfo = GetType().GetMethod("TheMethod")!;
        type = typeof(Tests);
        sourceFile = Path.Combine(directory, "NamingTests.cs");
    }

    [Fact]
    public Task Test()
    {
        return Verifier.Verify(BuildData())
            .AddScrubber(builder => builder.Replace('/', '\\'));
    }

    public Result Run(
        bool run,
        bool config,
        bool runVersion,
        bool arch,
        bool os,
        bool method,
        bool type,
        bool dir,
        bool fw,
        bool fwVersion)
    {
        var sharedNamer = VerifierSettings.SharedNamer;
        sharedNamer.UniqueForAssemblyConfiguration = false;
        sharedNamer.UniqueForRuntime = false;
        sharedNamer.UniqueForRuntimeAndVersion = false;
        sharedNamer.UniqueForArchitecture = false;
        sharedNamer.UniqueForOSPlatform = false;
        sharedNamer.UniqueForTargetFramework = false;
        sharedNamer.UniqueForTargetFrameworkAndVersion = false;

        var settings = new VerifySettings();

        if (run)
        {
            settings.UniqueForRuntime();
        }

        if (fw)
        {
            settings.UniqueForTargetFramework();
        }

        if (config)
        {
            settings.UniqueForAssemblyConfiguration();
        }

        if (runVersion)
        {
            settings.UniqueForRuntimeAndVersion();
        }

        if (fwVersion)
        {
            settings.UniqueForTargetFrameworkAndVersion();
        }

        if (arch)
        {
            settings.UniqueForArchitecture();
        }

        if (os)
        {
            settings.UniqueForOSPlatform();
        }

        if (method)
        {
            settings.UseMethodName("CustomMethod");
        }

        if (type)
        {
            settings.UseMethodName("CustomType");
        }

        if (dir)
        {
            settings.UseDirectory("customDir");
        }

        var builder = Builder(settings);
        var fileNames = builder.GetFileNames("txt");
        var fileNamesWithIndex = builder.GetIndexedFileNames("txt", 2);
        PrefixUnique.Clear();
        builder = Builder(settings);

        var receivedFiles = builder.ReceivedFiles.OrderBy(_ => x);
        var verifiedFiles = builder.VerifiedFiles.OrderBy(_ => x);
        PrefixUnique.Clear();
        return new()
        {
            FileNames = fileNames,
            FileNamesWithIndex = fileNamesWithIndex,
            ReceivedFiles = receivedFiles,
            VerifiedFiles = verifiedFiles
        };
    }

    public Result RunStatic(
        bool runG,
        bool configG,
        bool runVersionG,
        bool archG,
        bool osG,
        bool fwG,
        bool fwVersionG)
    {
        var sharedNamer = VerifierSettings.SharedNamer;
        sharedNamer.UniqueForAssemblyConfiguration = false;
        sharedNamer.UniqueForRuntime = false;
        sharedNamer.UniqueForRuntimeAndVersion = false;
        sharedNamer.UniqueForArchitecture = false;
        sharedNamer.UniqueForOSPlatform = false;
        sharedNamer.UniqueForTargetFramework = false;
        sharedNamer.UniqueForTargetFrameworkAndVersion = false;

        var settings = new VerifySettings();

        if (runG)
        {
            VerifierSettings.UniqueForRuntime();
        }

        if (fwG)
        {
            VerifierSettings.UniqueForTargetFramework();
        }

        if (configG)
        {
            VerifierSettings.UniqueForAssemblyConfiguration();
        }

        if (runVersionG)
        {
            VerifierSettings.UniqueForRuntimeAndVersion();
        }

        if (fwVersionG)
        {
            VerifierSettings.UniqueForTargetFrameworkAndVersion();
        }

        if (archG)
        {
            VerifierSettings.UniqueForArchitecture();
        }

        if (osG)
        {
            VerifierSettings.UniqueForOSPlatform();
        }

        var builder = Builder(settings);
        var fileNames = builder.GetFileNames("txt");
        var fileNamesWithIndex = builder.GetIndexedFileNames("txt", 2);
        PrefixUnique.Clear();
        builder = Builder(settings);

        var receivedFiles = builder.ReceivedFiles.OrderBy(_ => x);
        var verifiedFiles = builder.VerifiedFiles.OrderBy(_ => x);
        PrefixUnique.Clear();
        return new()
        {
            FileNames = fileNames,
            FileNamesWithIndex = fileNamesWithIndex,
            ReceivedFiles = receivedFiles,
            VerifiedFiles = verifiedFiles
        };
    }

    InnerVerifier Builder(VerifySettings settings)
    {
        GetFileConvention fileConvention = uniqueness => ReflectionFileNameBuilder.FileNamePrefix(methodInfo, type, sourceFile, settings, uniqueness);
        return new(sourceFile, settings, fileConvention);
    }

#pragma warning disable xUnit1013
    // Public method should be marked as test
    public void TheMethod()
    {

    }

    static bool[] bools = { true, false };

    public IEnumerable<Result> BuildData()
    {
        foreach (var runtime in bools)
        {
            foreach (var fw in bools)
            {
                foreach (var config in bools)
                {
                    foreach (var runtimeVersion in bools)
                    {
                        foreach (var fwVersion in bools)
                        {
                            foreach (var method in bools)
                            {
                                foreach (var type in bools)
                                {
                                    foreach (var dir in bools)
                                    {
                                        foreach (var arch in bools)
                                        {
                                            foreach (var os in bools)
                                            {
                                                yield return Run(
                                                    runtime,
                                                    config,
                                                    runtimeVersion,
                                                    arch,
                                                    os,
                                                    method,
                                                    type,
                                                    dir,
                                                    fw,
                                                    fwVersion
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerable<Result> BuildStaticData()
    {
        foreach (var runtimeStatic in bools)
        {
            foreach (var fwStatic in bools)
            {
                foreach (var configStatic in bools)
                {
                    foreach (var runtimeVersionStatic in bools)
                    {
                        foreach (var fwVersionStatic in bools)
                        {
                            foreach (var archStatic in bools)
                            {
                                foreach (var osStatic in bools)
                                {
                                    yield return RunStatic(
                                        runtimeStatic,
                                        configStatic,
                                        runtimeVersionStatic,
                                        archStatic,
                                        osStatic,
                                        fwStatic,
                                        fwVersionStatic
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class Result
    {
        public FilePair FileNames { get; set; }
        public FilePair FileNamesWithIndex { get; set; }
        public IOrderedEnumerable<string> ReceivedFiles { get; set; } = null!;
        public IOrderedEnumerable<string> VerifiedFiles { get; set; } = null!;
    }
}