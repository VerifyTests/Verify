﻿using System.Text;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public Task Verify(string input, VerifySettings? settings = null)
    {
        var builder = new StringBuilder(input);
        builder.FixNewlines();
        return Verify(builder, settings);
    }


    async Task Verify(StringBuilder input, VerifySettings? settings)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        var extension = settings.ExtensionOrTxt();
        var engine = new VerifyEngine(
            extension,
            settings,
            testType,
            directory,
            testName);

        var file = GetFileNames(extension, settings.Namer);

        ApplyScrubbers.Apply(input, settings.instanceScrubbers);
        var s = input.ToString();
        var result = await Comparer.Text(file, input, settings);
        engine.HandleCompareResult(result, file);
        await engine.ThrowIfRequired();
    }
}