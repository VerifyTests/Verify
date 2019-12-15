using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    public async Task VerifyBinary<T>(T input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        if (settings != null && settings.HasExtension())
        {
            if (SharedVerifySettings.TryGetConverter<T>(out var converter))
            {
                var converterSettings = new VerifySettings(settings);
                converterSettings.UseExtension(converter.ToExtension);
                var converterFunc = converter.Func(input!);
                await VerifyBinary(converterFunc, converterSettings);
                return;
            }
        }
        throw new Exception($"No converter found for {typeof(T).FullName}");
    }

    public async Task VerifyBinary(Stream input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        if (input.CanSeek)
        {
            input.Position = 0;
        }
        if (settings != null && settings.HasExtension())
        {
            if (SharedVerifySettings.TryGetConverter(settings.extension!, out var converter))
            {
                var converterSettings = new VerifySettings(settings);
                converterSettings.UseExtension(converter.ToExtension);
                await VerifyBinary(converter.Func(input), converterSettings);
                return;
            }
        }

        settings = settings.OrDefault();
        var extension = settings.ExtensionOrBin();
        var (receivedPath, verifiedPath) = GetFileNames(extension, settings.Namer);
        var verifyResult = await StreamVerifier.VerifyStreams(input, extension, receivedPath, verifiedPath);

        if (verifyResult == VerifyResult.MissingVerified)
        {
            throw VerificationNotFoundException(verifiedPath, exceptionBuilder);
        }

        if (verifyResult == VerifyResult.NotEqual)
        {
            var builder = new StringBuilder("Streams do not match.");
            builder.AppendLine();
            if (!BuildServerDetector.Detected)
            {
                builder.AppendLine("Verification command has been copied to the clipboard.");
            }

            throw exceptionBuilder(builder.ToString());
        }
    }

    public async Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings? settings = null)
    {
        settings = settings.OrDefault();
        var extension = settings.ExtensionOrBin();
        var missingVerified = new List<int>();
        var notEquals = new List<int>();
        var index = 0;
        foreach (var stream in streams)
        {
            var suffix = $"{index:D2}";
            var (receivedPath, verifiedPath) = GetFileNames(extension, settings.Namer, suffix);
            var verifyResult = await StreamVerifier.VerifyStreams(stream, extension, receivedPath, verifiedPath);

            if (verifyResult == VerifyResult.MissingVerified)
            {
                missingVerified.Add(index);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                notEquals.Add(index);
            }

            index++;
        }

        if (missingVerified.Count == 0 && notEquals.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder("Streams do not match.");
        builder.AppendLine();
        if (!BuildServerDetector.Detected)
        {
            builder.AppendLine("Verification command has been copied to the clipboard.");
        }

        if (missingVerified.Any())
        {
            builder.AppendLine($"Streams not verified: {string.Join(", ", missingVerified)}");
        }

        if (notEquals.Any())
        {
            builder.AppendLine($"Streams with differences: {string.Join(", ", notEquals)}");
        }

        throw exceptionBuilder(builder.ToString());
    }

    static Exception VerificationNotFoundException(string verifiedPath, Func<string, Exception> exceptionBuilder)
    {
        var verifiedFile = Path.GetFileName(verifiedPath);
        if (BuildServerDetector.Detected)
        {
            return exceptionBuilder($"First verification. {verifiedFile} not found.");
        }

        return exceptionBuilder($"First verification. {verifiedFile} not found. Verification command has been copied to the clipboard.");
    }
}