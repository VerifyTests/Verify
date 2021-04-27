using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VerifyTests;
using VerifyXunit;
using Xunit;
#if NET5_0 && DEBUG
using System.Linq;
#endif

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

[UsesVerify]
public class Tests
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddExtraDatetimeFormat("F");
        VerifierSettings.AddExtraDatetimeOffsetFormat("F");
    }

    [Fact]
    public async Task HttpResponseNested()
    {
        using HttpClient client = new();

        var result = await client.GetAsync("https://httpbin.org/get");

        await Verifier.Verify(new{result})
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "origin", "Content-Length", "TrailingHeaders");
    }

    [Fact]
    public async Task ImageHttpResponse()
    {
        using HttpClient client = new();

        var result = await client.GetAsync("https://httpbin.org/image/png");

        await Verifier.Verify(result)
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "origin", "Content-Length", "TrailingHeaders");
    }

    [Fact]
    public async Task HttpResponse()
    {
        using HttpClient client = new();

        var result = await client.GetAsync("https://httpbin.org/get");

        await Verifier.Verify(result)
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "origin", "Content-Length", "TrailingHeaders");
    }

#if NET5_0 && DEBUG

    [Fact]
    public async Task JsonGet()
    {
        HttpRecording.StartRecording();

        using HttpClient client = new();

        var result = await client.GetStringAsync("https://httpbin.org/get");

        await Verifier.Verify(result)
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "origin", "Content-Length");
    }

    [Fact]
    public async Task TestHttpRecordingWithResponse()
    {
        HttpRecording.StartRecording();

        using HttpClient client = new();

        var result = await client.GetStringAsync("https://httpbin.org/json");

        await Verifier.Verify(result)
            .ModifySerialization(settings =>
            {
                settings.IgnoreMembers("traceparent");
            });
    }

    #region HttpRecording

    [Fact]
    public async Task TestHttpRecording()
    {
        HttpRecording.StartRecording();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        await Verifier.Verify(
                new
                {
                    sizeOfResponse,
                })
            .ModifySerialization(settings =>
            {
                //scrub some headers that are not consistent between test runs
                settings.IgnoreMembers("traceparent", "Date");
            });
    }

    static async Task<int> MethodThatDoesHttpCalls()
    {
        using HttpClient client = new();

        var jsonResult = await client.GetStringAsync("https://httpbin.org/json");
        var xmlResult = await client.GetStringAsync("https://httpbin.org/xml");
        return jsonResult.Length + xmlResult.Length;
    }

    #endregion

    #region HttpRecordingExplicit

    [Fact]
    public async Task TestHttpRecordingExplicit()
    {
        HttpRecording.StartRecording();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        var httpCalls = HttpRecording.FinishRecording().ToList();

        // Ensure all calls finished in under 5 seconds
        var threshold = TimeSpan.FromSeconds(5);
        foreach (var call in httpCalls)
        {
            Assert.True(call.Duration < threshold);
        }

        await Verifier.Verify(
                new
                {
                    sizeOfResponse,
                    // Only use the Uri in the snapshot
                    httpCalls = httpCalls.Select(_ => _.Uri)
                });
    }

    #endregion

#endif

    [Fact]
    public Task WithNewline()
    {
        return Verifier.Verify(new {Property = "F\roo"});
    }

    #region LoggerRecordingTyped

    [Fact]
    public Task LoggingTyped()
    {
        var provider = LoggerRecording.Start();
        var logger = provider.CreateLogger<ClassThatUsesTypedLogging>();
        ClassThatUsesTypedLogging target = new(logger);

        var result = target.Method();

        return Verifier.Verify(result);
    }

    class ClassThatUsesTypedLogging
    {
        ILogger<ClassThatUsesTypedLogging> logger;

        public ClassThatUsesTypedLogging(ILogger<ClassThatUsesTypedLogging> logger)
        {
            this.logger = logger;
        }

        public string Method()
        {
            logger.LogWarning("The log entry");
            return "result";
        }
    }

    #endregion

    [Fact]
    public Task LoggingComplexState()
    {
        var provider = LoggerRecording.Start();
        provider.Log(LogLevel.Warning, default, new StateObject("Value1"), null, (_, _) => "The Message");
        using (provider.BeginScope(new StateObject("Value2")))
        {
            provider.Log(LogLevel.Warning, default, new StateObject("Value3"), null, (_, _) => "Entry in scope");
        }

        return Verifier.Verify("Foo");
    }

    class StateObject
    {
        public string Property { get; }

        public StateObject(string property)
        {
            Property = property;
        }
    }

    #region LoggerRecording

    [Fact]
    public Task Logging()
    {
        var provider = LoggerRecording.Start();
        ClassThatUsesLogging target = new(provider);

        var result = target.Method();

        return Verifier.Verify(result);
    }

    class ClassThatUsesLogging
    {
        ILogger logger;

        public ClassThatUsesLogging(ILogger logger)
        {
            this.logger = logger;
        }

        public string Method()
        {
            logger.LogWarning("The log entry");
            using (logger.BeginScope("The scope"))
            {
                logger.LogWarning("Entry in scope");
            }

            return "result";
        }
    }

    #endregion

    [ModuleInitializer]
    public static void TreatAsStringInit()
    {
        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, _) => target.Property);
    }

    [Fact]
    public Task TreatAsString()
    {
        return Verifier.Verify(new ClassWithToString {Property = "Foo"});
    }

    class ClassWithToString
    {
        public string Property { get; set; } = null!;
    }

    [Fact]
    public async Task OnVerifyMismatch()
    {
        VerifySettings settings = new();
        settings.DisableDiff();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        VerifierSettings.OnFirstVerify(
            filePair =>
            {
                if (filePair.Name.Contains("OnVerifyMismatch"))
                {
                    onFirstVerifyCalled = true;
                }

                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, _) =>
            {
                if (filePair.Name.Contains("OnVerifyMismatch"))
                {
                    Assert.NotEmpty(filePair.Received);
                    Assert.NotNull(filePair.Received);
                    Assert.NotEmpty(filePair.Verified);
                    Assert.NotNull(filePair.Verified);
                    onVerifyMismatchCalled = true;
                }

                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<VerifyException>(() => Verifier.Verify("value", settings));
        Assert.False(onFirstVerifyCalled);
        Assert.True(onVerifyMismatchCalled);
    }

    [Fact]
    public async Task OnFirstVerify()
    {
        VerifySettings settings = new();
        settings.DisableDiff();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        VerifierSettings.OnFirstVerify(
            filePair =>
            {
                if (filePair.Name.Contains("OnFirstVerify"))
                {
                    Assert.NotEmpty(filePair.Received);
                    Assert.NotNull(filePair.Received);
                    onFirstVerifyCalled = true;
                }

                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, _) =>
            {
                if (filePair.Name.Contains("OnFirstVerify"))
                {
                    onVerifyMismatchCalled = true;
                }

                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<VerifyException>(() => Verifier.Verify("value", settings));
        Assert.True(onFirstVerifyCalled);
        Assert.False(onVerifyMismatchCalled);
    }

    [ModuleInitializer]
    public static void SettingsArePassedInit()
    {
        VerifierSettings.RegisterStreamComparer(
            "SettingsArePassed",
            (_, _, _) => Task.FromResult(new CompareResult(true)));
    }

    [Fact]
    public async Task SettingsArePassed()
    {
        VerifySettings settings = new();
        settings.UseExtension("SettingsArePassed");
        await Verifier.Verify(new MemoryStream(new byte[] {1}), settings)
            .UseExtension("SettingsArePassed");
    }

    [Fact]
    public Task Throws()
    {
        return Verifier.Throws(MethodThatThrows);
    }

    static void MethodThatThrows()
    {
        throw new("The Message");
    }

    [Fact]
    public Task ThrowsNested()
    {
        return Verifier.Throws(Nested.MethodThatThrows);
    }

    static class Nested
    {
        public static void MethodThatThrows()
        {
            throw new("The Message");
        }
    }

    [Fact]
    public Task ThrowsArgumentException()
    {
        return Verifier.Throws(MethodThatThrowsArgumentException);
    }

    static void MethodThatThrowsArgumentException()
    {
        throw new ArgumentException("The Message", "The parameter");
    }

    [Fact]
    public Task ThrowsInheritedArgumentException()
    {
        return Verifier.Throws(MethodThatThrowsArgumentNullException);
    }

    static void MethodThatThrowsArgumentNullException()
    {
        throw new ArgumentNullException("The parameter", "The Message");
    }

    [Fact]
    public Task ThrowsAggregate()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntime();
        return Verifier.Throws(MethodThatThrowsAggregate, settings);
    }

    static void MethodThatThrowsAggregate()
    {
        throw new AggregateException(new Exception("The Message1"), new Exception("The Message2"));
    }

    [Fact]
    public Task ThrowsTask()
    {
        return Verifier.ThrowsTask(TaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");
    }

    static Task TaskMethodThatThrows()
    {
        throw new("The Message");
    }

    [Fact]
    public Task ThrowsTaskGeneric()
    {
        return Verifier.ThrowsTask(TaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");
    }

    static Task<string> TaskMethodThatThrowsGeneric()
    {
        throw new("The Message");
    }

    [Fact]
    public Task ThrowsValueTask()
    {
        return Verifier.ThrowsValueTask(ValueTaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");
    }

    static ValueTask ValueTaskMethodThatThrows()
    {
        throw new("The Message");
    }

    [Fact]
    public Task ThrowsValueTaskGeneric()
    {
        return Verifier.ThrowsValueTask(ValueTaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");
    }

    static ValueTask<string> ValueTaskMethodThatThrowsGeneric()
    {
        throw new("The Message");
    }

    [Fact]
    public Task StringBuilder()
    {
        return Verifier.Verify(new StringBuilder("value"));
    }

    [Fact]
    public Task NestedStringBuilder()
    {
        return Verifier.Verify(new {StringBuilder = new StringBuilder("value")});
    }

    [Fact]
    public Task TextWriter()
    {
        StringWriter target = new();
        target.Write("content");
        return Verifier.Verify(target);
    }

    [Fact]
    public Task NestedTextWriter()
    {
        StringWriter target = new();
        target.Write("content");
        return Verifier.Verify(new {target});
    }

    [Fact]
    public async Task StringWithDifferingNewline()
    {
        var fullPath = Path.GetFullPath("../../../Tests.StringWithDifferingNewline.verified.txt");
        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\r\nb");
        await Verifier.Verify("a\r\nb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\rb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\nb");
        FileNameBuilder.ClearPrefixList();

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\nb");
        await Verifier.Verify("a\r\nb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\rb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\nb");
        FileNameBuilder.ClearPrefixList();

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\rb");
        await Verifier.Verify("a\r\nb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\rb");
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("a\nb");
    }

    [Fact]
    public Task Stream()
    {
        return Verifier.Verify(new MemoryStream(new byte[] {1}));
    }

    [Fact]
    public Task StreamNotAtStart()
    {
        MemoryStream stream = new(new byte[] {1, 2, 3, 4});
        stream.Position = 2;
        return Verifier.Verify(stream);
    }

    [Fact]
    public Task StreamNotAtStartAsText()
    {
        MemoryStream stream = new(Encoding.UTF8.GetBytes("foo"));
        stream.Position = 2;
        return Verifier.Verify(stream).UseExtension("txt");
    }

    [Fact]
    public Task Streams()
    {
        return Verifier.Verify(
            new List<Stream>
            {
                new MemoryStream(new byte[] {1}),
                new MemoryStream(new byte[] {2})
            });
    }

    [Fact]
    public Task StreamsWithNull()
    {
        return Verifier.Verify(
            new List<Stream?>
            {
                new MemoryStream(new byte[] {1}),
                null
            });
    }

    [Fact]
    public async Task ShouldNotIgnoreCase()
    {
        await Verifier.Verify("A");
        VerifySettings settings = new();
        settings.DisableDiff();
        FileNameBuilder.ClearPrefixList();
        await Assert.ThrowsAsync<VerifyException>(() => Verifier.Verify("a", settings));
    }

    [Fact]
    public Task Newlines()
    {
        return Verifier.Verify("a\r\nb\nc\rd\r\n");
    }

    class Element
    {
        public string? Id { get; set; }
    }

    [Fact]
    public Task ShouldThrowForExtensionOnSerialization()
    {
        VerifySettings settings = new();
        settings.UseExtension("json");
        settings.UseMethodName("Foo");
        settings.ModifySerialization(_ => _.IgnoreMember("StackTrace"));
        settings.DisableDiff();

        Element element = new();
        return Verifier.ThrowsTask(() => Verifier.Verify(element, settings))
            .ModifySerialization(_ => _.IgnoreMember("StackTrace"));
    }

    [Fact]
    public Task StringExtension()
    {
        VerifySettings settings = new();
        settings.UseExtension("xml");

        return Verifier
            .Verify("<a>b</a>", settings);
    }

    [Fact]
    public Task TaskResult()
    {
        var target = Task.FromResult("value");
        return Verifier.Verify(target);
    }

    static async IAsyncEnumerable<string> AsyncEnumerableMethod()
    {
        await Task.Delay(1);
        yield return "one";
        await Task.Delay(1);
        yield return "two";
    }

    [Fact]
    public Task AsyncEnumerable()
    {
        return Verifier.Verify(AsyncEnumerableMethod());
    }

    static async IAsyncEnumerable<DisposableTarget> AsyncEnumerableDisposableMethod(DisposableTarget target)
    {
        await Task.Delay(1);
        yield return target;
    }

    [Fact]
    public async Task AsyncEnumerableDisposable()
    {
        DisposableTarget target = new();
        await Verifier.Verify(AsyncEnumerableDisposableMethod(target));
        Assert.True(target.Disposed);
    }

    static async IAsyncEnumerable<AsyncDisposableTarget> AsyncEnumerableAsyncDisposableMethod(AsyncDisposableTarget target)
    {
        await Task.Delay(1);
        yield return target;
    }

    [Fact]
    public async Task AsyncEnumerableAsyncDisposable()
    {
        AsyncDisposableTarget target = new();
        await Verifier.Verify(AsyncEnumerableAsyncDisposableMethod(target));
        Assert.True(target.AsyncDisposed);
    }

    [Fact]
    public async Task TaskResultAsyncDisposable()
    {
        AsyncDisposableTarget disposableTarget = new();
        var target = Task.FromResult(disposableTarget);
        await Verifier.Verify(target);
        Assert.True(disposableTarget.AsyncDisposed);
    }

    class AsyncDisposableTarget :
        IAsyncDisposable,
        IDisposable
    {
#pragma warning disable 414
        public string Property = "Value";
#pragma warning restore 414
        public bool AsyncDisposed;

        public ValueTask DisposeAsync()
        {
            AsyncDisposed = true;
            return new();
        }

        public void Dispose()
        {
            throw new();
        }
    }

    [Fact]
    public async Task TaskResultDisposable()
    {
        DisposableTarget disposableTarget = new();
        var target = Task.FromResult(disposableTarget);
        await Verifier.Verify(target);
        Assert.True(disposableTarget.Disposed);
    }

    class DisposableTarget :
        IDisposable
    {
#pragma warning disable 414
        public string Property = "Value";
#pragma warning restore 414
        public bool Disposed;

        public void Dispose()
        {
            Disposed = true;
        }
    }

#if !NETFRAMEWORK
    [Fact]
    public async Task VerifyBytesAsync()
    {
        VerifySettings settings = new();
        settings.UseExtension("jpg");
        await Verifier.Verify(File.ReadAllBytesAsync("sample.jpg"), settings);
    }
#endif

    [Fact]
    public async Task VerifyFilePath()
    {
        await Verifier.VerifyFile("sample.txt");
        Assert.False(FileEx.IsFileLocked("sample.txt"));
    }

    //[Fact(Skip = "explicit")]
    //public async Task ShouldUseExtraSettings()
    //{
    //    ApplyExtraSettings(settings => { settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat; });

    //    var person = new Person
    //    {
    //        Dob = new DateTime(1980, 5, 5, 1, 1, 1)
    //    };
    //    DontScrubDateTimes();
    //    await Verify(person);
    //}
}