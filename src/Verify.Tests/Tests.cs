using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

[UsesVerify]
public class Tests
{
    static Tests()
    {
        VerifierSettings.AddExtraDatetimeFormat("F");
        VerifierSettings.AddExtraDatetimeOffsetFormat("F");
    }


    [Fact]
    public Task TreatAsString()
    {
        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, _) => target.Property);
        return Verifier.Verify(new ClassWithToString {Property = "Foo"});
    }

    class ClassWithToString
    {
        public string Property { get; set; } = null!;
    }

    [Fact]
    public async Task OnVerifyMismatch()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.DisableClipboard();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        settings.OnFirstVerify(
            _ =>
            {
                onFirstVerifyCalled = true;
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (filePair, _) =>
            {
                Assert.NotEmpty(filePair.Received);
                Assert.NotNull(filePair.Received);
                Assert.NotEmpty(filePair.Verified);
                Assert.NotNull(filePair.Verified);
                onVerifyMismatchCalled = true;
                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("value", settings));
        Assert.False(onFirstVerifyCalled);
        Assert.True(onVerifyMismatchCalled);
    }

    [Fact]
    public async Task OnFirstVerify()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.DisableClipboard();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        settings.OnFirstVerify(
            filePair =>
            {
                Assert.NotEmpty(filePair.Received);
                Assert.NotNull(filePair.Received);
                onFirstVerifyCalled = true;
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (_, _) =>
            {
                onVerifyMismatchCalled = true;
                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("value", settings));
        Assert.True(onFirstVerifyCalled);
        Assert.False(onVerifyMismatchCalled);
    }

    [Fact]
    public async Task SettingsArePassed()
    {
        VerifierSettings.RegisterComparer(
            "SettingsArePassed",
            (_, _, _) => Task.FromResult(new CompareResult(true)));
        var settings = new VerifySettings();
        settings.UseExtension("SettingsArePassed");
        await Verifier.Verify(new MemoryStream(new byte[] {1}), settings).UseExtension("SettingsArePassed");
    }

    [Fact]
    public Task Throws()
    {
        return Verifier.Throws(MethodThatThrows);
    }

    static void MethodThatThrows()
    {
        throw new Exception("The Message");
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
        var settings = new VerifySettings();
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
        return Verifier.ThrowsAsync(TaskMethodThatThrows);
    }

    static Task TaskMethodThatThrows()
    {
        throw new Exception("The Message");
    }

    [Fact]
    public Task ThrowsValueTask()
    {
        return Verifier.ThrowsAsync(ValueTaskMethodThatThrows);
    }

    static ValueTask ValueTaskMethodThatThrows()
    {
        throw new Exception("The Message");
    }

    [Fact]
    public async Task StringWithDifferingNewline()
    {
        var fullPath = Path.GetFullPath("../../../Tests.StringWithDifferingNewline.verified.txt");
        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\r\nb");
        await Verifier.Verify("a\r\nb");
        await Verifier.Verify("a\rb");
        await Verifier.Verify("a\nb");

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\nb");
        await Verifier.Verify("a\r\nb");
        await Verifier.Verify("a\rb");
        await Verifier.Verify("a\nb");

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\rb");
        await Verifier.Verify("a\r\nb");
        await Verifier.Verify("a\rb");
        await Verifier.Verify("a\nb");
    }

    [Fact]
    public Task Stream()
    {
        return Verifier.Verify(new MemoryStream(new byte[] {1}));
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
        var settings = new VerifySettings();
        settings.DisableClipboard();
        settings.DisableDiff();
        await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("a", settings));
    }

    [Fact]
    public Task Newlines()
    {
        return Verifier.Verify("a\r\nb\nc\rd\r\n");
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
        var target = new DisposableTarget();
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
        var target = new AsyncDisposableTarget();
        await Verifier.Verify(AsyncEnumerableAsyncDisposableMethod(target));
        Assert.True(target.AsyncDisposed);
    }

    [Fact]
    public async Task TaskResultAsyncDisposable()
    {
        var disposableTarget = new AsyncDisposableTarget();
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
            return new ValueTask();
        }

        public void Dispose()
        {
            throw new Exception();
        }
    }

    [Fact]
    public async Task TaskResultDisposable()
    {
        var disposableTarget = new DisposableTarget();
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
        var settings = new VerifySettings();
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