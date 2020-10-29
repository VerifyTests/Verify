using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public Task VerifyXml(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "xml", settings, sourceFile);
        }

        public Task VerifyJson(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "json", settings, sourceFile);
        }

        public Task VerifyHtml(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "html", settings, sourceFile);
        }

        public async Task VerifyFileContent(
            string content,
            string extension,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            settings.UseExtension(extension);
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(content);
        }
    }
}