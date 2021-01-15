using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Theory]
    [ClassData(typeof(TestData))]
    public Task Test(
        bool run,
        bool runG,
        bool config,
        bool configG,
        bool runVersion,
        bool runVersionG)
    {
        VerifierSettings.SharedNamer.UniqueForAssemblyConfiguration = false;
        VerifierSettings.SharedNamer.UniqueForRuntime = false;
        VerifierSettings.SharedNamer.UniqueForRuntimeAndVersion = false;
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

        var builder = new FileNameBuilder(
            GetType().GetMethod("TheMethod")!,
            typeof(Tests),
            directory,
            Path.Combine(directory, "NamingTests.cs"),
            null,
            settings);
        var fileNames = builder.GetFileNames("txt");
        var fileNamesWithIndex = builder.GetFileNames("txt", 2);
        File.WriteAllText(fileNames.Received,"");
        File.WriteAllText(fileNames.Verified,"");
        File.WriteAllText(fileNamesWithIndex.Received,"");
        File.WriteAllText(fileNamesWithIndex.Verified,"");
        var receivedFiles = builder.GetReceivedFiles();
        var verifiedFiles = builder.GetVerifiedFiles();
        FileNameBuilder.ClearPrefixList();
        return Verifier.Verify(new
            {
                fileNames,
                fileNamesWithIndex,
                receivedFiles,
                verifiedFiles
            })
            .UseParameters(
                run,
                runG,
                config,
                configG,
                runVersion,
                runVersionG)
            .UseMethodName("_")
            .UseTypeName("_")
            .AddScrubber(_ => _.Replace('/','\\'));
    }

#pragma warning disable xUnit1013
    // Public method should be marked as test
    public void TheMethod()
    {

    }
    public class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var bools = new[]{true,false};
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

                                    yield return new object[] {
                                        runtime,
                                        runtimeStatic,
                                        config,
                                        configStatic,
                                        runtimeVersion,
                                        runtimeVersionStatic};
                                }
                            }
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}