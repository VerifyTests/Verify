public class VerifyCombinationsTests
{
    #region GlobalCaptureExceptions

    [ModuleInitializer]
    public static void Initialize() =>
        VerifyCombinationSettings.CaptureExceptions();

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
        return Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return Combination(captureExceptions: false)
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
        });
    }

    public static string BuildAddress(int streetNumber, string street, string city)
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