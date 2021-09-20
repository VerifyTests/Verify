﻿using System.Linq;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Fact]
    public Task Test()
    {
        return Verifier.Verify(BuildData())
            .AddScrubber(builder => builder.Replace('/', '\\'));
    }

    public Result Run(
        bool run,
        bool runG,
        bool config,
        bool configG,
        bool runVersion,
        bool runVersionG,
        bool arch,
        bool archG,
        bool os,
        bool osG,
        bool method,
        bool type,
        bool dir)
    {
        var sharedNamer = VerifierSettings.SharedNamer;
        sharedNamer.UniqueForAssemblyConfiguration = false;
        sharedNamer.UniqueForRuntime = false;
        sharedNamer.UniqueForRuntimeAndVersion = false;
        sharedNamer.UniqueForArchitecture = false;
        sharedNamer.UniqueForOSPlatform = false;

        var directory = Path.Combine(Path.GetTempPath(), "VerifyNamer");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);

        var settings = new VerifySettings();

        if (run)
        {
            settings.UniqueForRuntime();
        }

        if (runG)
        {
            VerifierSettings.UniqueForRuntime();
        }

        if (config)
        {
            settings.UniqueForAssemblyConfiguration();
        }

        if (configG)
        {
            VerifierSettings.UniqueForAssemblyConfiguration();
        }

        if (runVersion)
        {
            settings.UniqueForRuntimeAndVersion();
        }

        if (runVersionG)
        {
            VerifierSettings.UniqueForRuntimeAndVersion();
        }

        if (arch)
        {
            settings.UniqueForArchitecture();
        }

        if (archG)
        {
            VerifierSettings.UniqueForArchitecture();
        }

        if (os)
        {
            settings.UniqueForOSPlatform();
        }

        if (osG)
        {
            VerifierSettings.UniqueForOSPlatform();
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

        var builder = Builder(directory, settings);
        var fileNames = builder.GetFileNames("txt");
        var fileNamesWithIndex = builder.GetIndexedFileNames("txt", 2);
        File.WriteAllText(fileNames.Received, "");
        File.WriteAllText(fileNames.Verified, "");
        File.WriteAllText(fileNamesWithIndex.Received, "");
        File.WriteAllText(fileNamesWithIndex.Verified, "");
        PrefixUnique.Clear();
        builder = Builder(directory, settings);

        var receivedFiles = builder.ReceivedFiles.OrderBy(x => x);
        var verifiedFiles = builder.VerifiedFiles.OrderBy(x => x);
        PrefixUnique.Clear();
        return new()
        {
            FileNames = fileNames,
            FileNamesWithIndex = fileNamesWithIndex,
            ReceivedFiles = receivedFiles,
            VerifiedFiles = verifiedFiles
        };
    }

    InnerVerifier Builder(string directory, VerifySettings settings)
    {
        var methodInfo = GetType().GetMethod("TheMethod")!;
        var type = typeof(Tests);
        var sourceFile = Path.Combine(directory, "NamingTests.cs");
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
            foreach (var runtimeStatic in bools)
            {
                foreach (var config in bools)
                {
                    foreach (var configStatic in bools)
                    {
                        foreach (var runtimeVersion in bools)
                        {
                            foreach (var runtimeVersionStatic in bools)
                            {
                                foreach (var method in bools)
                                {
                                    foreach (var type in bools)
                                    {
                                        foreach (var dir in bools)
                                        {
                                            foreach (var arch in bools)
                                            {
                                                foreach (var archStatic in bools)
                                                {
                                                    foreach (var os in bools)
                                                    {
                                                        foreach (var osStatic in bools)
                                                        {
                                                            yield return Run(
                                                                runtime,
                                                                runtimeStatic,
                                                                config,
                                                                configStatic,
                                                                runtimeVersion,
                                                                runtimeVersionStatic,
                                                                arch,
                                                                archStatic,
                                                                os,
                                                                osStatic,
                                                                method,
                                                                type,
                                                                dir
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