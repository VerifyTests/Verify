using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        Task VerifyString(string target, VerifySettings settings)
        {
            var builder = new StringBuilder(target);
            builder.FixNewlines();
            return Verify(builder, settings);
        }

        async Task Verify(StringBuilder target, VerifySettings settings)
        {
            Guard.AgainstNull(target, nameof(target));
            var extension = settings.ExtensionOrTxt();
            ApplyScrubbers.Apply(target, settings.instanceScrubbers);

            var engine = new VerifyEngine(
                extension,
                settings,
                directory,
                testName,
                assembly);

            var list = VerifierSettings.GetFileAppenders(settings).ToList();
            FilePair file;
            if (list.Any())
            {
                file = GetFileNames(extension, settings.Namer, "01");
            }
            else
            {
                file = GetFileNames(extension, settings.Namer);
            }

            var result = await Comparer.Text(file, target, settings);
            engine.HandleCompareResult(result, file);

            for (var index = 0; index < list.Count; index++)
            {
                var stream = list[index];
                var conversionFile = GetFileNames(stream.Extension, settings.Namer, $"{index + 2:D2}");
                await ProcessConversionStream(settings, conversionFile, stream, engine);
            }

            await engine.ThrowIfRequired();
        }
    }
}