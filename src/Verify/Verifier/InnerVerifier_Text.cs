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
            StringBuilder builder = new(target);
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

            VerifyEngine engine = new(extension, settings, directory, testName, assembly);

            List<ResultBuilder> builders = new()
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