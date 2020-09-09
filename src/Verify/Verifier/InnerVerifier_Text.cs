using System.Collections.Generic;
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

            var builders = new List<ResultBuilder>
            {
                new ResultBuilder(extension, file => Comparer.Text(file, target, settings))
            };

            builders.AddRange(VerifierSettings.GetFileAppenders(settings)
                .Select(appender =>
                {
                    return new ResultBuilder(
                        appender.Extension,
                        file => GetResult(settings, file, appender));
                }));

            await HandleResults(settings, builders, engine);

            await engine.ThrowIfRequired();
        }

        async Task HandleResults(VerifySettings settings, List<ResultBuilder> results, VerifyEngine engine)
        {
            if (results.Count == 1)
            {
                var item = results[0];
                var file = GetFileNames(item.Extension, settings.Namer);
                var result = await item.GetResult(file);
                engine.HandleCompareResult(result, file);
                return;
            }

            for (var index = 0; index < results.Count; index++)
            {
                var item = results[index];
                var file = GetFileNames(item.Extension, settings.Namer, $"{index:D2}");
                var result = await item.GetResult(file);
                engine.HandleCompareResult(result, file);
            }
        }
    }
}