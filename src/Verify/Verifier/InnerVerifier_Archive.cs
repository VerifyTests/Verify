using System.IO.Compression;

namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyArchive(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include,
        object? info,
        FileScrubber? scrubber)
    {
        var targets = new List<Target>(1);
        if (info is not null)
        {
            targets.Add(new(
                VerifierSettings.TxtOrJson,
                JsonFormatter.AsJson(
                    settings,
                    counter,
                    info)));
        }

        include ??= _ => true;

        foreach (var entry in archive.Entries)
        {
            if (!include(entry))
            {
                continue;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entry.FullName);
            targets.Add(await TargetFromFile(entry.FullName, fileNameWithoutExtension, scrubber, ()=> entry.Open()));
        }

        return await VerifyInner(targets);
    }
}