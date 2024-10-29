// ReSharper disable MemberCanBePrivate.Global
#if NET8_0_OR_GREATER
public class VerifyCombinationsTests
{
    public static async Task<string> TaskMethod(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);
        await Task.Delay(1);
        return $"{streetNumber} {street}, {city}";
    }

    [Fact]
    public Task TaskTest()
    {
        int[] streetNumbers = [1, 10];
        string[] streets = ["Smith St", "Wallace St"];
        string[] cities = ["Sydney", "Chicago"];
        return Combination()
            .Verify(
                TaskMethod,
                streetNumbers,
                streets,
                cities);
    }

    public static async ValueTask<string> ValueTaskMethod(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);
        await Task.Delay(1);
        return $"{streetNumber} {street}, {city}";
    }

    [Fact]
    public Task ValueTaskTest()
    {
        int[] streetNumbers = [1, 10];
        string[] streets = ["Smith St", "Wallace St"];
        string[] cities = ["Sydney", "Chicago"];
        return Combination()
            .Verify(
                ValueTaskMethod,
                streetNumbers,
                streets,
                cities);
    }

    public static string SimpleReturnMethod(int streetNumber, string street, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);

        return $"{streetNumber} {street}, {city}";
    }

    [Fact]
    public Task SimpleReturnTest()
    {
        int[] streetNumbers = [1, 10];
        string[] streets = ["Smith St", "Wallace St"];
        string[] cities = ["Sydney", "Chicago"];
        return Combination()
            .Verify(
                SimpleReturnMethod,
                streetNumbers,
                streets,
                cities);
    }
}
#endif