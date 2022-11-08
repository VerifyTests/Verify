// Non-nullable field is uninitialized.

using System.Runtime.InteropServices;

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

    [Theory]
    [InlineData("a")]
    public Task ReplaceInvalidParamChar(string value) =>
        Verify("foo")
            .UseParameters(Path.GetInvalidPathChars().First());

    [Theory]
    [InlineData(1, 2)]
    public Task ParameterCount_TooFew(int one, int two) =>
        Verify("Value").UseParameters(1);

    [Theory]
    [InlineData(1, 2)]
    public async Task IncorrectParameterCount_TooMany(int one, int two)
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify("Value").UseParameters(1, 2, 3));
        Assert.Equal("The number of passed in parameters (3) must be fewer than the number of parameters for the method (2).", exception.Message);
    }

    [Theory]
    [InlineData(1000.9999d)]
    public async Task LocalizedParam(decimal value)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        try
        {
            await Verify(value)
                .UseParameters(value);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }

    [ModuleInitializer]
    public static void TreatAsStringInit() =>
        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, _) => target.Property);

    [Fact]
    public Task TreatAsString() =>
        Verify(new ClassWithToString
        {
            Property = "Foo"
        });

    class ClassWithToString
    {
        public string Property { get; set; } = null!;
    }

    [Fact]
    // ReSharper disable once IdentifierTypo
    public Task MisMatchcase()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // No way to caseless File.Exists https://github.com/dotnet/core/issues/4596 on linux
            return Task.CompletedTask;
        }

        return Verify("Value");
    }

    [Fact]
    public async Task OnVerifyMismatch()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        VerifierSettings.OnFirstVerify(
            (filePair, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnVerifyMismatch"))
                {
                    onFirstVerifyCalled = true;
                }

                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnVerifyMismatch"))
                {
                    Assert.NotEmpty(filePair.ReceivedPath);
                    Assert.NotNull(filePair.ReceivedPath);
                    Assert.NotEmpty(filePair.VerifiedPath);
                    Assert.NotNull(filePair.VerifiedPath);
                    onVerifyMismatchCalled = true;
                }

                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
        Assert.False(onFirstVerifyCalled);
        Assert.True(onVerifyMismatchCalled);
    }

#if NET6_0_OR_GREATER

    [Fact]
    public async Task OnFirstVerify()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        var onFirstVerifyCalled = false;
        var onVerifyMismatchCalled = false;
        VerifierSettings.OnFirstVerify(
            (filePair, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnFirstVerify"))
                {
                    Assert.NotEmpty(filePair.ReceivedPath);
                    Assert.NotNull(filePair.ReceivedPath);
                    onFirstVerifyCalled = true;
                }

                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnFirstVerify"))
                {
                    onVerifyMismatchCalled = true;
                }

                return Task.CompletedTask;
            });
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
        Assert.True(onFirstVerifyCalled);
        Assert.False(onVerifyMismatchCalled);
    }

#endif

    [ModuleInitializer]
    public static void SettingsArePassedInit() =>
        VerifierSettings.RegisterStreamComparer(
            "SettingsArePassed",
            (_, _, _) => Task.FromResult(new CompareResult(true)));

    [Fact]
    public Task StringBuilder() =>
        Verify(new StringBuilder("value"));

    [Fact]
    public Task NestedStringBuilder() =>
        Verify(new
        {
            StringBuilder = new StringBuilder("value")
        });

    [Fact]
    public Task TextWriter()
    {
        var target = new StringWriter();
        target.Write("content");
        return Verify(target);
    }

    [Fact]
    public Task NestedTextWriter()
    {
        var target = new StringWriter();
        target.Write("content");
        return Verify(new
        {
            target
        });
    }

    [Fact]
    public async Task ShouldNotIgnoreCase()
    {
        await Verify("A");
        var settings = new VerifySettings();
        settings.DisableDiff();
        PrefixUnique.Clear();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", settings));
    }

    [Fact]
    public async Task DanglingFiles()
    {
        var receivedFile = CurrentFile.Relative($"Tests.DanglingFiles.{Namer.RuntimeAndVersion}.received.txt");
        var verifiedOldFile = CurrentFile.Relative($"Tests.DanglingFiles.{Namer.RuntimeAndVersion}.01.verified.txt");
        var verifiedNewFile = CurrentFile.Relative($"Tests.DanglingFiles.{Namer.RuntimeAndVersion}#01.verified.txt");
        File.WriteAllText(receivedFile, "");
        File.WriteAllText(verifiedOldFile, "");
        File.WriteAllText(verifiedNewFile, "");
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(verifiedOldFile));
        Assert.False(File.Exists(verifiedNewFile));
    }

    [Theory]
    [InlineData("param")]
    public async Task DanglingFilesIgnoreParametersForVerified(string param)
    {
        var receivedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParametersForVerified_param=param.{Namer.RuntimeAndVersion}.01.received.txt");
        var verifiedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParametersForVerified.{Namer.RuntimeAndVersion}.01.verified.txt");
        File.WriteAllText(receivedFile, "");
        File.WriteAllText(verifiedFile, "");
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .IgnoreParametersForVerified(param)
            .AutoVerify();
        await Task.Delay(1000);
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(verifiedFile));
    }

    class Element
    {
        public string? Id { get; set; }
    }

#if NET6_0_OR_GREATER

    [Fact]
    public async Task StringWithUtf8Bom()
    {
        var utf8 = Encoding.UTF8;
        var preamble = utf8.GetString(utf8.GetPreamble());
        await Verify($"{preamble}a").AutoVerify();
        await Verify("a").DisableRequireUniquePrefix();
    }

#endif

    [Fact]
    public async Task EnsureUtf8BomPreamble()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        var file = CurrentFile.Relative($"Tests.EnsureUtf8BomPreamble.{Namer.RuntimeAndVersion}.verified.txt");
        File.Delete(file);
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        var fileBytes = File.ReadAllBytes(file).Take(3);
        var preambleBytes = Encoding.UTF8.GetPreamble();
        Assert.Equal(preambleBytes, fileBytes);
    }

    [Fact]
    public Task StringExtension() =>
        Verify("<a>b</a>", "xml");

    [Fact]
    public Task TaskResult()
    {
        var target = Task.FromResult<string>("value");
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
    public Task AsyncEnumerable() =>
        Verify(AsyncEnumerableMethod());

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

    [Fact]
    public async Task Result()
    {
        #region VerifyResult

        var result = await Verify(
            new
            {
                Property = "Value To Check"
            });
        Assert.Contains("Value To Check", result.Text);

        #endregion

        Assert.NotNull(result.Target);
    }

    [Fact]
    public async Task ResultAutoVerify()
    {
        var result = await Verify("Value")
            .AutoVerify();

        Assert.Single(result.Files);
    }

    [Fact]
    public async Task ResultAutoVerifyMissingVerified()
    {
        if (ContinuousTestingDetector.Detected)
        {
            return;
        }
        try
        {
            var result = await Verify("Value")
                .UniqueForRuntimeAndVersion()
                .AutoVerify();

            Assert.Single(result.Files);
        }
        finally
        {
            var file = CurrentFile.Relative($"Tests.ResultAutoVerifyMissingVerified.{Namer.RuntimeAndVersion}.verified.txt");
            File.Delete(file);
        }
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
            return new();
        }

        public void Dispose() =>
            throw new();
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

        public void Dispose() =>
            Disposed = true;
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