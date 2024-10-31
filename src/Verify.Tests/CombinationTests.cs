// ReSharper disable MemberCanBePrivate.Global
public class CombinationTests
{
    static int[] params1 = [1, 10];
    static string[] params2 = ["Smith St", "Wallace St"];

    public static async IAsyncEnumerable<string> AsyncEnumerableMethod(int param1, string param2)
    {
        await Task.Delay(1);
        yield return $"{param1} {param2}";
    }

    [Fact]
    public Task AsyncEnumerableTest() =>
        Combination()
            .Verify(
                AsyncEnumerableMethod,
                params1,
                params2);

    public static IEnumerable<string> EnumerableMethod(int param1, string param2)
    {
        yield return $"{param1} {param2}";
    }

    [Fact]
    public Task EnumerableTest() =>
        Combination()
            .Verify(
                EnumerableMethod,
                params1,
                params2);

    public static async Task<string> TaskMethod(int param1, string param2)
    {
        await Task.Delay(1);
        return $"{param1} {param2}";
    }

    [Fact]
    public Task TaskTest() =>
        Combination()
            .Verify(
                TaskMethod,
                params1,
                params2);

    public static Task VoidTaskMethod(int param1, string param2) =>
        Task.Delay(1);

    [Fact]
    public Task VoidTaskTest() =>
        Combination()
            .Verify(
                VoidTaskMethod,
                params1,
                params2);

    public static async ValueTask<string> ValueTaskMethod(int param1, string param2)
    {
        await Task.Delay(1);
        return $"{param1} {param2}";
    }

    [Fact]
    public Task ValueTaskTest() =>
        Combination()
            .Verify(
                ValueTaskMethod,
                params1,
                params2);

    public static async ValueTask VoidValueTaskMethod(int param1, string param2) =>
        await Task.Delay(1);

    [Fact]
    public Task VoidValueTaskTest() =>
        Combination()
            .Verify(
                VoidValueTaskMethod,
                params1,
                params2);

    public static string SimpleReturnMethod(int param1, string param2) =>
        $"{param1} {param2}";

    [Fact]
    public Task SimpleReturnTest() =>
        Combination()
            .Verify(
                SimpleReturnMethod,
                params1,
                params2);
}