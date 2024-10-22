#if NET8_0_OR_GREATER
public class VerifyCombinationsSample
{
    #region CombinationTargetMethod

    public string BuildAddress(int streetNumber, string street, string city)
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
        int[] streetNumbers = [-1, 0, 10];
        string[] streets = [null!, "", " ", "Wallace St"];
        string[] cities = [null!, "", " ", "Sydney"];
        return VerifyCombinations(
            BuildAddress,
            streetNumbers,
            streets,
            cities);
    }

    #endregion
}
#endif