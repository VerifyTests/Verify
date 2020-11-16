using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        Task VerifyString(string target)
        {
            var appenders = VerifierSettings.GetJsonAppenders(settings);
            var builder = new StringBuilder(target);
            builder.FixNewlines();
            if (appenders.Any())
            {
                return SerializeAndVerify(builder.ToString(), appenders);
            }

            return VerifyStringBuilder(builder);
        }

        async Task VerifyStringBuilder(StringBuilder target)
        {
            var extension = settings.ExtensionOrTxt();
            ApplyScrubbers.Apply(target, settings.instanceScrubbers);

            var engine = new VerifyEngine(extension, settings, directory, testName, assembly);

            var builders = new List<ResultBuilder>
            {
                new(extension, file => Comparer.Text(file, target, settings))
            };

            builders.AddRange(VerifierSettings.GetFileAppenders(settings)
                .Select(appender =>
                {
                    return new ResultBuilder(
                        appender.Extension,
                        file => GetResult(settings, file, appender));
                }));

            await HandleResults(builders, engine);

            await engine.ThrowIfRequired();
        }
    }
}