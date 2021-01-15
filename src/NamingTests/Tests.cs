using System;
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
        bool runtime,
        bool runtimeG,
        bool config,
        bool configG,
        bool runtimeVersion,
        bool runtimeVersionG)
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var settings = new VerifySettings();
        if (runtime)
        {
            settings.UniqueForRuntime();
        }
        if (runtimeG)
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
        if (runtimeVersion)
        {
            settings.UniqueForRuntimeAndVersion();
        }
        if (runtimeVersionG)
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
        FileNameBuilder.ClearPrefixList();
        return Verifier.Verify(new
            {
                fileNames,
                fileNamesWithIndex
            })
            .UseParameters(
                runtime,
                runtimeG,
                config,
                configG,
                runtimeVersion,
                runtimeVersionG)
            .UseMethodName("_")
            .UseTypeName("_");
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