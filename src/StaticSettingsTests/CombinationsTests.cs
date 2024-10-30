public class CombinationsTests
{
    #region GlobalCaptureExceptions

    [ModuleInitializer]
    public static void Initialize() =>
        CombinationSettings.CaptureExceptions();

    #endregion

    [Fact]
    public Task Defaults()
    {
        string[] list = ["A", "b", "C"];
        return Combination().Verify(
            _ => _.ToLower(),
            list);
    }

    [Fact]
    public async Task CallbacksTest()
    {
        var beforeCalled = false;
        IReadOnlyList<object?>? beforeKeys = null;
        object? afterResult = null;
        IReadOnlyList<object?>? afterKeys = null;
        IReadOnlyList<object?>? exceptionKeys = null;
        Exception? exceptionResult = null;
        var afterCalled = false;
        var exceptionCalled = false;
        int[] params1 = [1, 10];
        string[] params2 = ["value1", "value2"];
        CombinationSettings.UseCallbacks(
            keys =>
            {
                beforeKeys = keys;
                beforeCalled = true;
                return Task.CompletedTask;
            },
            (keys, result) =>
            {
                afterResult = result;
                afterKeys = keys;
                afterCalled = true;
                return Task.CompletedTask;
            },
            (keys, exception) =>
            {
                exceptionResult = exception;
                exceptionKeys = keys;
                exceptionCalled = true;
                return Task.CompletedTask;
            });
        await Combination()
            .Verify(
                (param1, param2) => $"{param1} {param2}",
                params1,
                params2);
        Assert.True(beforeCalled);
        Assert.True(afterCalled);
        Assert.False(exceptionCalled);
        await Verify(new
            {
                beforeKeys,
                afterKeys,
                afterResult,
                exceptionKeys,
                exceptionResult
            })
            .UseMethodName("CallbackResults");
    }

    [Fact]
    public async Task ExceptionCallbacksTest()
    {
        var beforeCalled = false;
        IReadOnlyList<object?>? beforeKeys = null;
        object? afterResult = null;
        IReadOnlyList<object?>? afterKeys = null;
        IReadOnlyList<object?>? exceptionKeys = null;
        Exception? exceptionResult = null;
        var afterCalled = false;
        var exceptionCalled = false;
        int[] params1 = [1, 10];
        string[] params2 = ["value1", "value2"];
        CombinationSettings.UseCallbacks(
            keys =>
            {
                beforeKeys = keys;
                beforeCalled = true;
                return Task.CompletedTask;
            },
            (keys, result) =>
            {
                afterResult = result;
                afterKeys = keys;
                afterCalled = true;
                return Task.CompletedTask;
            },
            (keys, exception) =>
            {
                exceptionResult = exception;
                exceptionKeys = keys;
                exceptionCalled = true;
                return Task.CompletedTask;
            });

        static string Method(int i, string s) =>
            throw new("Message");

        await Combination()
            .Verify(
                Method,
                params1,
                params2);
        Assert.True(beforeCalled);
        Assert.False(afterCalled);
        Assert.True(exceptionCalled);
        await Verify(new
            {
                beforeKeys,
                afterKeys,
                afterResult,
                exceptionKeys,
                exceptionResult
            })
            .UseMethodName("ExceptionCallbackResults")
            .IgnoreStackTrace();
    }

    [Fact]
    public Task WithCaptureExceptions()
    {
        string[] a = ["A", "b", "C"];
        return Combination(captureExceptions: true)
            .Verify(
                a =>
                {
                    if (a == "b")
                    {
                        throw new ArgumentException("B is not allowed");
                    }

                    return a.ToLower();
                },
                a);
    }

    [Fact]
    public Task WithNoCaptureExceptions()
    {
        string[] a = ["A", "b", "C"];
        return Assert.ThrowsAsync<ArgumentException>(
            () => Combination(captureExceptions: false)
                .Verify(
                    a =>
                    {
                        if (a == "b")
                        {
                            throw new ArgumentException("B is not allowed");
                        }

                        return a.ToLower();
                    },
                    a));
    }

    static string BuildAddress(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);

        return $"{streetNumber} {street}, {city}";
    }

    #region CombinationSample_CaptureExceptionsFalse

    [Fact]
    public Task BuildAddressExceptionsDisabledTest()
    {
        int[] streetNumbers = [1, 10];
        string[] streets = ["Smith St", "Wallace St"];
        string[] cities = ["Sydney", "Chicago"];
        return Combination(captureExceptions: false)
            .Verify(
                BuildAddress,
                streetNumbers,
                streets,
                cities);
    }

    #endregion

    #region CombinationSample_CustomSerializationModuleInitializer

    static CustomCombinationConverter customConverter = new();

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddExtraSettings(_ => _.Converters.Insert(0, customConverter));

    #endregion

    #region CombinationSample_CustomSerialization

    [Fact]
    public Task Combination_CustomSerialization()
    {
        int[] streetNumbers = [1, 10];
        string[] streets = ["Smith St", "Wallace St"];
        string[] cities = ["Sydney", "Chicago"];
        return Combination()
            .Verify(
                BuildAddress,
                streetNumbers,
                streets,
                cities);
    }

    #endregion

    #region CombinationSample_CustomSerializationConverter

    class CustomCombinationConverter :
        CombinationResultsConverter
    {
        protected override string BuildPropertyName(IReadOnlyList<CombinationKey> keys) =>
            string.Join(", ", keys.Select(_ => _.Value));
    }

    #endregion
}