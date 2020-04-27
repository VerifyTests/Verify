using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

public class Tests :
    VerifyBase
{
    static Tests()
    {
        SharedVerifySettings.AddExtraDatetimeFormat("F");
        SharedVerifySettings.AddExtraDatetimeOffsetFormat("F");
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
            receivedFile =>
            {
                onFirstVerifyCalled = true;
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (receivedFile, verifiedFile) =>
            {
                Assert.NotEmpty(receivedFile);
                Assert.NotNull(receivedFile);
                Assert.NotEmpty(verifiedFile);
                Assert.NotNull(verifiedFile);
                onVerifyMismatchCalled = true;
                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<XunitException>(() => Verify("value", settings));
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
            receivedFile =>
            {
                Assert.NotEmpty(receivedFile);
                Assert.NotNull(receivedFile);
                onFirstVerifyCalled = true;
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (receivedFile, verifiedFile) =>
            {
                onVerifyMismatchCalled = true;
                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<XunitException>(() => Verify("value", settings));
        Assert.True(onFirstVerifyCalled);
        Assert.False(onVerifyMismatchCalled);
    }


    [Fact]
    public async Task SettingsArePassed()
    {
        VerifySettings? fromGlobal = null;
        SharedVerifySettings.RegisterComparer(
            "SettingsArePassed",
            (verifySettings, received, verified) =>
            {
                fromGlobal = verifySettings;
                return Task.FromResult(new CompareResult(true));
            });
        var settings = new VerifySettings();
        settings.UseExtension("SettingsArePassed");
        await Verify(new MemoryStream(new byte[] {1}), settings);
        Assert.Same(fromGlobal, settings);
    }

    [Fact]
    public Task NewlinesText()
    {
        return Verify("a\r\nb\nc");
    }

    [Fact]
    public async Task ShouldNotIgnoreCase()
    {
        await Verify("A");
        var settings = new VerifySettings();
        settings.DisableClipboard();
        settings.DisableDiff();
        await Assert.ThrowsAsync<XunitException>(() => Verify("a", settings));
    }

    [Fact]
    public Task Newlines()
    {
        return Verify("a\r\nb\nc");
    }

    [Fact]
    public Task TaskResult()
    {
        var target = Task.FromResult("value");
        return Verify(target);
    }

    static async IAsyncEnumerable<string> AsyncEnumerableMethod()
    {
        await Task.Delay(1);
        yield return "one";
        await Task.Delay(1);
        yield return "two";
    }

    [Fact]
    public async Task AsyncEnumerable()
    {
        await Verify(AsyncEnumerableMethod());
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
        await Verify(AsyncEnumerableDisposableMethod(target));
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
        await Verify(AsyncEnumerableAsyncDisposableMethod(target));
        Assert.True(target.AsyncDisposed);
    }

    [Fact]
    public async Task TaskResultAsyncDisposable()
    {
        var disposableTarget = new AsyncDisposableTarget();
        var target = Task.FromResult(disposableTarget);
        await Verify(target);
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
        await Verify(target);
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


#if NETCOREAPP3_1

    [Fact]
    public async Task VerifyBytesAsync()
    {
        var settings = new VerifySettings();
        settings.UseExtension("jpg");
        await Verify(File.ReadAllBytesAsync("sample.jpg"), settings);
    }

#endif

    [Fact]
    public async Task VerifyFilePath()
    {
        await VerifyFile("sample.txt");
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

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}