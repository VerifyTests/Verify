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
            return VerifyBinary(Enumerable.Empty<ConversionStream>(),target, null);
        }

        async Task VerifyStringBuilder(StringBuilder target, string extension)
        {
            ApplyScrubbers.Apply(target, settings.instanceScrubbers);

            VerifyEngine engine = new(settings, fileNameBuilder);

            var received = target.ToString();
            List<ResultBuilder> builders = new()
            {
                new(extension, file => Comparer.Text(file, received, settings))
            };

            builders.AddRange(
                VerifierSettings.GetFileAppenders(settings)
                    .Select(appender =>
                    {
                        return new ResultBuilder(
                            appender.Extension,
                            file => GetResult(settings, file, appender));
                    }));

            await engine.HandleResults(builders);

            await engine.ThrowIfRequired();
        }
    }
}