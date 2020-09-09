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
            return VerifyStringBuilder(builder, settings);
        }

        async Task VerifyStringBuilder(StringBuilder target, VerifySettings settings)
        {
            var extension = settings.ExtensionOrTxt();
            ApplyScrubbers.Apply(target, settings.instanceScrubbers);

            var engine = new VerifyEngine(extension, settings, directory, testName, assembly);

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
    }
}