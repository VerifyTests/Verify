using ApprovalTests.Core;

namespace ApprovalTests;

public class Approvals
{
    [Obsolete(@"Use Verifier.VerifyFile(receivedFilePath)", true)]
    public static void VerifyFile(string receivedFilePath)
    {
    }

    [Obsolete(@"Use Verifier.VerifyFile(receivedFilePath)", true)]
    public static void Verify(FileInfo receivedFilePath)
    {
    }

    [Obsolete("See https://github.com/VerifyTests/Verify/blob/main/docs/naming.md", true)]
    public static void RegisterDefaultNamerCreation(Func<IApprovalNamer> creator)
    {
    }

    [Obsolete("See https://github.com/VerifyTests/Verify/blob/main/docs/naming.md", true)]
    public static void UseAssemblyLocationForApprovedFiles()
    {
    }

    [Obsolete(@"Use Verifier.Verify(text)", true)]
    public static void Verify(object text)
    {
    }

    [Obsolete(@"Use Verifier.Verify(text)
.AddScrubber(scrubber)", true)]
    public static void Verify(string text, Func<string, string>? scrubber = null)
    {
    }

    [Obsolete(@"Use Verifier.Verify(text)
.UseExtension(fileExtensionWithDot)
.AddScrubber(scrubber)", true)]
    public static void VerifyWithExtension(string text, string fileExtensionWithDot, Func<string, string>? scrubber = null)
    {
    }

    [Obsolete("Use Verifier.Throws(...) or Verifier.Verify(Exception)", true)]
    public static void VerifyException(Exception e)
    {
    }

    [Obsolete("Use Verifier.Throws(...) or Verifier.Verify(Exception)", true)]
    public static void VerifyExceptionWithStacktrace(Exception e)
    {
    }

    [Obsolete("Pass the enumerable and the header to Verifier.Verify<T>(T) using an anon type https://github.com/VerifyTests/Verify/blob/main/docs/anonymous-types.md", true)]
    public static void VerifyAll<T>(string header, IEnumerable<T> enumerable, string label)
    {
    }

    [Obsolete("Use Verifier.Verify<T>(T)", true)]
    public static void VerifyAll<T>(IEnumerable<T> enumerable, string label)
    {
    }

    [Obsolete("Run the formatter then pass the result to Verifier.Verify<T>(T)", true)]
    public static void VerifyAll<T>(IEnumerable<T> enumerable, string label, Func<T, string> formatter)
    {
    }

    [Obsolete("Run the formatter then pass the result and the header to Verifier.Verify<T>(T) using an anon type https://github.com/VerifyTests/Verify/blob/main/docs/anonymous-types.md", true)]
    public static void VerifyAll<T>(string header, IEnumerable<T> enumerable, Func<T, string> formatter)
    {
    }

    [Obsolete("Run the formatter then pass the result to Verifier.Verify<T>(T)", true)]
    public static void VerifyAll<T>(IEnumerable<T> enumerable, Func<T, string> formatter)
    {
    }

    [Obsolete("Use Verifier.Verify<T>(T)", true)]
    public static void VerifyAll<K, V>(IDictionary<K, V> dictionary)
    {
    }

    [Obsolete("Pass the dictionary and the header to Verifier.Verify<T>(T) using an anon type https://github.com/VerifyTests/Verify/blob/main/docs/anonymous-types.md", true)]
    public static void VerifyAll<K, V>(string header, IDictionary<K, V> dictionary)
    {
    }

    [Obsolete("Run the formatter then pass the result and the header to Verifier.Verify<T>(T) using an anon type https://github.com/VerifyTests/Verify/blob/main/docs/anonymous-types.md", true)]
    public static void VerifyAll<K, V>(string header, IDictionary<K, V> dictionary, Func<K, V, string> formatter)
    {
    }

    [Obsolete("Run the formatter then pass the result to Use Verifier.Verify<T>(T)", true)]
    public static void VerifyAll<K, V>(IDictionary<K, V> dictionary, Func<K, V, string> formatter)
    {
        VerifyAll(dictionary.OrderBy(p => p.Key), p => formatter(p.Key, p.Value));
    }

    [Obsolete("Use Verifier.Verify(byte[]).UseExtension(\"ext\")", true)]
    public static void VerifyBinaryFile(byte[] bytes, string fileExtensionWithDot)
    {
    }

    [Obsolete("Use Verifier.Verify(string).UseExtension(\"html\")", true)]
    public static void VerifyHtml(string html)
    {
    }

    [Obsolete("Use Verifier.Verify(string).UseExtension(\"xml\")", true)]
    public static void VerifyXml(string xml)
    {
    }

    [Obsolete("Use Verifier.VerifyJson(string)", true)]
    public static void VerifyJson(string json)
    {
    }

    [Obsolete("See https://github.com/VerifyTests/Verify.Syncfusion or https://github.com/VerifyTests/Verify.DocNet or https://github.com/VerifyTests/Verify.Aspose", true)]
    public static void VerifyPdfFile(string pdfFilePath)
    {
    }
}