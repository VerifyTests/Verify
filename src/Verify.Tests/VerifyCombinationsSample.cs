// ReSharper disable MemberCanBePrivate.Global
#if NET8_0_OR_GREATER
public class VerifyCombinationsSample
{
    #region CombinationTargetMethod

    public static string BuildAddress(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);

        return $"{streetNumber} {street}, {city}";
    }

    #endregion

    #region CombinationSample

    [Fact]
    public Task BuildAddressTest()
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

    #region CombinationAlignment

    [Fact]
    public Task CombinationAlignment()
    {
        int[] numbers = [1, 100];
        string[] strings = ["a", "bbbb"];
        Date?[] dates = [new(2020, 10, 1), null, Date.MinValue];
        return Combination()
            .Verify(
                (number, text, date) => string.Join(" ", number, text, date?.ToString("yyyy-MM-dd")),
                numbers,
                strings,
                dates);
    }

    #endregion

    #region CombinationSample_CaptureExceptions

    [Fact]
    public Task BuildAddressExceptionsTest()
    {
        int[] streetNumbers = [-1, 0, 10];
        string[] streets = ["", " ", "Valid St"];
        string[] cities = [null!, "Valid City"];
        return Combination(captureExceptions: true)
            .Verify(
                BuildAddress,
                streetNumbers,
                streets,
                cities
            );
    }

    #endregion
}
#endif