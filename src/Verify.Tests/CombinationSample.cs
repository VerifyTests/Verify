// ReSharper disable MemberCanBePrivate.Global
#if NET8_0_OR_GREATER
public class CombinationSample
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
        int[] streetNumber = [1, 10];
        string[] street = ["Smith St", "Wallace St"];
        string[] city = ["Sydney", "Chicago"];
        return Combination()
            .Verify(
                BuildAddress,
                streetNumber,
                street,
                city);
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
        int[] streetNumber = [-1, 0, 10];
        string[] street = ["", " ", "Valid St"];
        string[] city = [null!, "Valid City"];
        return Combination(captureExceptions: true)
            .Verify(
                BuildAddress,
                streetNumber,
                street,
                city
            );
    }

    #endregion
}
#endif