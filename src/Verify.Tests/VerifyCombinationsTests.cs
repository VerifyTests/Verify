// ReSharper disable MemberCanBePrivate.Global
#if NET8_0_OR_GREATER
public class VerifyCombinationsTests
{
    public static async Task<string> TaskMethod(int param1, string param2)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(param2);
        ArgumentOutOfRangeException.ThrowIfLessThan(param1, 1);
        await Task.Delay(1);
        return $"{param1} {param2}";
    }

    [Fact]
    public Task TaskTest()
    {
        int[] params1 = [1, 10];
        string[] params2 = ["Smith St", "Wallace St"];
        return Combination()
            .Verify(
                TaskMethod,
                params1,
                params2);
    }

    public static async ValueTask<string> ValueTaskMethod(int param1, string param2)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(param2);
        ArgumentOutOfRangeException.ThrowIfLessThan(param1, 1);
        await Task.Delay(1);
        return $"{param1} {param2}";
    }

    [Fact]
    public Task ValueTaskTest()
    {
        int[] param1 = [1, 10];
        string[] param2 = ["Smith St", "Wallace St"];
        return Combination()
            .Verify(
                ValueTaskMethod,
                param1,
                param2);
    }

    public static string SimpleReturnMethod(int param1, string param2)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(param2);
        ArgumentOutOfRangeException.ThrowIfLessThan(param1, 1);

        return $"{param1} {param2}";
    }

    [Fact]
    public Task SimpleReturnTest()
    {
        int[] param1 = [1, 10];
        string[] param2 = ["Smith St", "Wallace St"];
        return Combination()
            .Verify(
                SimpleReturnMethod,
                param1,
                param2);
    }
}
#endif