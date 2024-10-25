#pragma warning disable VerifyCombinations
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
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }

    [Fact]
    public Task WithCaptureExceptions()
    {
        string[] a = ["A", "b", "C"];
        return VerifyCombinations(
            a =>
            {
                if (a == "b")
                {
                    throw new ArgumentException("B is not allowed");
                }

                return a.ToLower();
            },
            a,
            captureExceptions: true);
    }

    [Fact]
    public Task WithNoCaptureExceptions()
    {
        string[] a = ["A", "b", "C"];
        return Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return VerifyCombinations(
                a =>
                {
                    if (a == "b")
                    {
                        throw new ArgumentException("B is not allowed");
                    }

                    return a.ToLower();
                },
                a,
                captureExceptions: false);
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
        return VerifyCombinations(
            BuildAddress,
            streetNumbers,
            streets,
            cities,
            captureExceptions: false);
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
        return VerifyCombinations(
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