#if NET8_0_OR_GREATER
public class VerifyCombinationsSample
{
    public string BuildAddress(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        if (streetNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(streetNumber));
        }

        return $"{streetNumber} {street}, {city}";
    }

    [Fact]
    public Task BuildAddressTest()
    {
        List<int> streetNumbers = [-1, 0, 10];
        List<string> streets = [null!, "", " ", "Wallace St"];
        List<string> cities = [null!, "", " ", "Sydney"];
        return VerifyCombinations(
            BuildAddress,
            streetNumbers,
            streets,
            cities);
    }
}
#endif