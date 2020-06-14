using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Verify(string target, VerifySettings? settings = null)
        {
            var builder = new StringBuilder(target);
            builder.FixNewlines();
            return Verify(builder, settings);
        }

        async Task Verify(StringBuilder target, VerifySettings? settings)
        {
            Guard.AgainstNull(target, nameof(target));
            settings = settings.OrDefault();

            var extension = settings.ExtensionOrTxt();
            var engine = new VerifyEngine(
                extension,
                settings,
                directory,
                testName,
                assembly);

            var file = GetFileNames(extension, settings.Namer);

            ApplyScrubbers.Apply(target, settings.instanceScrubbers);
            var s = target.ToString();
            var result = await Comparer.Text(file, target, settings);
            engine.HandleCompareResult(result, file);
            await engine.ThrowIfRequired();
        }
    }
}