using System.Threading.Tasks;
using VerifyTests;

namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        public Task VerifyXml(
            string content,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyXml(content, settings, sourceFile);
        }

        public Task VerifyJson(
            string content,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyJson(content, settings, sourceFile);
        }

        public Task VerifyHtml(
            string content,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyHtml(content, settings, sourceFile);
        }

        public Task VerifyFileContent(
            string content,
            string extension,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyFileContent(content, extension, settings, sourceFile);
        }
    }
}