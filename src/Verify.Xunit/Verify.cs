
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextCopy;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VerifyXunit
{
    public class VerifyBase :
        XunitContextBase
    {
        public async Task Verify(string target)
        {
            var (receivedPath, verifiedPath) = GetFileNames();
            if (!File.Exists(verifiedPath))
            {
                await FileHelpers.WriteText(receivedPath, target);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                throw new Exception($"First verification. {Context.UniqueTestName}.verified.txt not found. Verification command has been copied to the clipboard.");
            }

            var verifiedText = await FileHelpers.ReadText(verifiedPath);

            try
            {
                Assert.Equal(verifiedText, target);
            }
            catch (EqualException exception)
            {
                await FileHelpers.WriteText(receivedPath, target);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                var fieldInfo = exception.GetType().GetField("message",BindingFlags.Instance|BindingFlags.NonPublic);
                fieldInfo.SetValue(exception,exception.Message);

                throw ;
            }
        }

        (string receivedPath, string verifiedPath) GetFileNames()
        {
            var filePrefix = Path.Combine(SourceDirectory, Context.UniqueTestName);
            var receivedPath = $"{filePrefix}.received.txt";
            var verifiedPath = $"{filePrefix}.verified.txt";
            return (receivedPath, verifiedPath);
        }

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }
    }
}

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();
    public static void Append(string received, string verified)
    {
        var temp = $"cmd /c move /Y \"{received}\" \"{verified}\"";
        lock (builder)
        {
            builder.AppendLine(temp);
            Clipboard.SetText(builder.ToString());
        }
    }
}