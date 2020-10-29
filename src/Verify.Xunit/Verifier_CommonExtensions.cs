using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static Task VerifyJson(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "json", settings, sourceFile);
        }

        public static Task VerifyHtml(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "html", settings, sourceFile);
        }

        public static Task VerifyXml(
            string content,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyFileContent(content, "xml", settings, sourceFile);
        }

        public static async Task VerifyFileContent(
            string content,
            string extension,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            settings.UseExtension(extension);
            using var verifier = GetVerifier(settings, sourceFile);
            await verifier.Verify(content);
        }
    }
}