// Non-nullable field is uninitialized.

// ReSharper disable UnusedParameter.Local
#pragma warning disable CS8618

public class Tests
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddExtraDateTimeFormat("F");
        VerifierSettings.AddExtraDateTimeOffsetFormat("F");
    }

    [Theory]
    [InlineData("a")]
    public Task ReplaceInvalidParamChar(string value) =>
        Verify("foo")
            .UseParameters(Path
                .GetInvalidPathChars()
                .First());

    [Theory]
    [InlineData(1, 2)]
    public Task ParameterCount_TooFew(int one, int two) =>
        Verify("Value")
            .UseParameters(1);

    [Theory]
    [InlineData(1, 2)]
    public async Task IncorrectParameterCount_TooMany(int one, int two)
    {
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            Verify("Value")
                .UseParameters(1, 2, 3));
        Assert.Equal("The number of passed in parameters (3) must not exceed the number of parameters for the method (2).", exception.Message);
    }

    // [Theory]
    // [InlineData(1000.9999d)]
    // public async Task LocalizedParam(decimal value)
    // {
    //     var thread = Thread.CurrentThread;
    //     var culture = thread.CurrentCulture;
    //     thread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
    //     try
    //     {
    //         await Verify(value)
    //             .UseParameters(value).ConfigureAwait(true);
    //     }
    //     finally
    //     {
    //         thread.CurrentCulture = culture;
    //     }
    // }

    [ModuleInitializer]
    public static void TreatAsStringInit() =>
        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, _) => target.Property);

    [Fact]
    public Task TreatAsString() =>
        Verify(new ClassWithToString("Foo"));

    record ClassWithToString(string Property);

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

    static bool onFirstVerifyCalled2;
    static bool onVerifyMismatchCalled2;

    [ModuleInitializer]
    public static void OnVerifyMismatchInit()
    {
        VerifierSettings.OnFirstVerify(
            (filePair, _, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnVerifyMismatch"))
                {
                    onFirstVerifyCalled2 = true;
                }

                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, _, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnVerifyMismatch"))
                {
                    Assert.NotEmpty(filePair.ReceivedPath);
                    Assert.NotNull(filePair.ReceivedPath);
                    Assert.NotEmpty(filePair.VerifiedPath);
                    Assert.NotNull(filePair.VerifiedPath);
                    onVerifyMismatchCalled2 = true;
                }

                return Task.CompletedTask;
            });
    }

    [Fact]
    public async Task OnVerifyMismatch()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
        Assert.False(onFirstVerifyCalled2);
        Assert.True(onVerifyMismatchCalled2);
    }

    [Fact]
    public async Task OnCallbacksTest()
    {
        var onVerifyBeforeCalled = false;
        var onVerifyAfterCalled = false;
        var settings = new VerifySettings();
        settings.OnVerify(
            before: () => onVerifyBeforeCalled = true,
            after: () => onVerifyAfterCalled = true);

        await Verify("value", settings);
        Assert.True(onVerifyBeforeCalled);
        Assert.True(onVerifyAfterCalled);
    }

    #region OnInstanceHandlers

    [Fact]
    public Task OnCallbacks()
    {
        var settings = new VerifySettings();
        settings.OnVerify(
            before: () => Debug.WriteLine("before"),
            after: () => Debug.WriteLine("after"));
        settings.OnFirstVerify(
            (receivedFile, receivedText, autoVerify) =>
            {
                Debug.WriteLine(receivedFile);
                Debug.WriteLine(receivedText);
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (filePair, message, autoVerify) =>
            {
                Debug.WriteLine(filePair.ReceivedPath);
                Debug.WriteLine(filePair.VerifiedPath);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });

        return Verify("value", settings);
    }

    #endregion

    #region OnFluentHandlers

    [Fact]
    public Task OnFluentCallbacks() =>
        Verify("value")
            .OnVerify(
                before: () => Debug.WriteLine("before"),
                after: () => Debug.WriteLine("after"))
            .OnFirstVerify(
                (receivedFile, receivedText, autoVerify) =>
                {
                    Debug.WriteLine(receivedFile);
                    Debug.WriteLine(receivedText);
                    return Task.CompletedTask;
                })
            .OnVerifyMismatch(
                (filePair, message, autoVerify) =>
                {
                    Debug.WriteLine(filePair.ReceivedPath);
                    Debug.WriteLine(filePair.VerifiedPath);
                    Debug.WriteLine(message);
                    return Task.CompletedTask;
                });

    #endregion

    // ReSharper restore UnusedParameter.Local

#if NET6_0_OR_GREATER
    static bool onFirstVerifyCalled;
    static bool onVerifyMismatchCalled;

    [ModuleInitializer]
    public static void OnFirstVerifyInit()
    {
        VerifierSettings.OnFirstVerify(
            (filePair, _, _) =>
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
            (filePair, _, _) =>
            {
                if (filePair.VerifiedPath.Contains("OnFirstVerify"))
                {
                    onVerifyMismatchCalled = true;
                }

                return Task.CompletedTask;
            });
    }

    [Fact]
    public async Task OnFirstVerify()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
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
        var verifiedNewFile = CurrentFile.Relative($"Tests.DanglingFiles.{Namer.RuntimeAndVersion}#01.verified.txt");
        await File.WriteAllTextAsync(receivedFile, "");
        await File.WriteAllTextAsync(verifiedNewFile, "");
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(verifiedNewFile));
    }

    [Theory]
    [InlineData("param")]
    public async Task DanglingFilesIgnoreParametersForVerified(string param)
    {
        var receivedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParametersForVerified_param=param.{Namer.RuntimeAndVersion}#01.received.txt");
        var verifiedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParametersForVerified.{Namer.RuntimeAndVersion}#01.verified.txt");
        await File.WriteAllTextAsync(receivedFile, "");
        await File.WriteAllTextAsync(verifiedFile, "");
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .IgnoreParametersForVerified(param)
            .AutoVerify();
        await Task.Delay(1000);
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(verifiedFile));
    }

#if NET9_0

    [Theory]
    [InlineData("P1", "P2")]
    public async Task DanglingFilesIgnoreParameters(string param1, string param2)
    {
        var receivedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParameters_param1=P1_param2=P2.{Namer.RuntimeAndVersion}#01.received.txt");
        var verifiedFile = CurrentFile.Relative($"Tests.DanglingFilesIgnoreParameters_param2=P2.{Namer.RuntimeAndVersion}#01.verified.txt");
        await File.WriteAllTextAsync(receivedFile, "");
        await File.WriteAllTextAsync(verifiedFile, "");
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .UseParameters(param1, param2)
            .IgnoreParameters(nameof(param1))
            .AutoVerify();
        await Task.Delay(1000);
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(verifiedFile));
    }

#endif

#if NET6_0_OR_GREATER
    [Fact]
    public async Task StringWithUtf8Bom()
    {
        var utf8 = Encoding.UTF8;
        var preamble = utf8.GetString(utf8.GetPreamble());
        await Verify($"{preamble}a")
            .AutoVerify();
        await Verify("a")
            .DisableRequireUniquePrefix();
    }

#endif

    [Fact]
    public async Task EnsureUtf8BomPreamble()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        var file = CurrentFile.Relative($"Tests.{nameof(EnsureUtf8BomPreamble)}.{Namer.RuntimeAndVersion}.verified.txt");
        File.Delete(file);
        await Verify("value")
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        var fileBytes = (await File.ReadAllBytesAsync(file))
            .Take(3);
        var preambleBytes = Encoding.UTF8.GetPreamble();
        Assert.Equal(preambleBytes, fileBytes);
        File.Delete(file);
    }

    [Fact]
    public Task StringExtension() =>
        Verify("<a>b</a>", "xml");

    [Fact]
    public Task FuncOfTaskResult()
    {
        static async Task<string> Target()
        {
            await Task.Delay(1);
            return "value";
        }

        return Verify(Target);
    }

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
    public async Task FuncOfTaskResultAsyncDisposable()
    {
        var disposableTarget = new AsyncDisposableTarget();
        var target = async () =>
        {
            await Task.Delay(1);
            return disposableTarget;
        };
        await Verify(target);
        Assert.True(disposableTarget.AsyncDisposed);
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
    public async Task FuncOfTaskResultDisposable()
    {
        var disposableTarget = new DisposableTarget();

        async Task<DisposableTarget> Target()
        {
            await Task.Delay(1);
            return disposableTarget;
        }

        await Verify(Target);
        Assert.True(disposableTarget.Disposed);
    }

    [Fact]
    public async Task TaskResultDisposable()
    {
        var disposableTarget = new DisposableTarget();
        var target = Task.FromResult(disposableTarget);
        await Verify(target);
        Assert.True(disposableTarget.Disposed);
    }

    [Fact]
    public async Task ThrowForTerminatedFluent()
    {
        var settingsTask = Verify("value");
        await settingsTask;

        Exception? exception = null;
        try
        {
            // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS4014
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            settingsTask.IncludeObsoletes();
#pragma warning restore CS4014
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception);
        Assert.Equal("This SettingsTask instance has already been converted to a Task and can no longer be modified. Conversion to a Task occurs either through awaiting the instance or calling ToTask.", exception.Message);
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