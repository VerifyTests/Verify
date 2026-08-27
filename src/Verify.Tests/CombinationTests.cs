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

    [Fact]
    public async Task EmptyListThrowsDescriptive()
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Combination()
                .Verify((int _) => "x", []));
        Assert.Contains("empty", exception.Message);
    }

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

    [Fact]
    public Task RecordingTest()
    {
        Recording.Start();
        return Combination()
            .Verify(
                (param1, param2) =>
                {
                    Recording.Add("key", $"recorded {param1} {param2}");
                    return SimpleReturnMethod(param1, param2);
                },
                params1,
                params2);
    }

    [Fact]
    public Task RecordingWithExceptionTest()
    {
        Recording.Start();
        return Combination(captureExceptions: true)
            .Verify(
                (param1, param2) =>
                {
                    Recording.Add("key", $"recorded {param1} {param2}");
                    if (param1 == 1)
                    {
                        throw new("boom");
                    }

                    return $"{param1} {param2}";
                },
                params1,
                params2)
            .IgnoreStackTrace();
    }

    [Fact]
    public Task RecordingWithExceptionPausedTest()
    {
        Recording.Start();
        return Combination(captureExceptions: true)
            .Verify(
                (param1, param2) =>
                {
                    Recording.Add("key", $"recorded {param1} {param2}");
                    Recording.Pause();
                    if (param1 == 1)
                    {
                        throw new("boom");
                    }

                    return $"{param1} {param2}";
                },
                params1,
                params2)
            .IgnoreStackTrace();
    }

    static string ThrowMultiLine(int value) =>
        throw new ArgumentException(
            """
            Value cannot be null.
            Parameter name: p
            """);

    // ArgumentException messages are flattened onto one line, so the parts of the
    // message need a separator between them
    [Fact]
    public Task MultiLineArgumentException() =>
        Combination(captureExceptions: true)
            .Verify(
                ThrowMultiLine,
                params1);

    // Only reachable by constructing the results directly, since the runner requires
    // every list to have at least one item
    [Fact]
    public Task EmptyResults() =>
        Verify(new CombinationResults([], [], null));

    // an unclosed object would swallow everything written after it
    [Fact]
    public Task EmptyResultsNested() =>
        Verify(
            new
            {
                results = new CombinationResults([], [], null),
                after = "TheValue"
            });

    [Fact]
    public Task RecordingPausedTest()
    {
        Recording.Start();
        return Combination()
            .Verify(
                (param1, param2) =>
                {
                    Recording.Add("key", $"recorded {param1} {param2}");
                    Recording.Pause();
                    return SimpleReturnMethod(param1, param2);
                },
                params1,
                params2);
    }

    // DateTime.Equals ignores Kind and DateTimeOffset.Equals compares only the instant,
    // while the rendered names include both, so keys that differ only in those must not
    // share a cached name
    [Fact]
    public Task DateKeysThatCompareEqual() =>
        Combination()
            .Verify(
                (dateTime, dateTimeOffset) => $"{dateTime.Kind} {dateTimeOffset.Offset}",
                new List<DateTime>
                {
                    new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new(2000, 1, 1, 0, 0, 0, DateTimeKind.Local),
                    new(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                },
                new List<DateTimeOffset>
                {
                    new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    new(2000, 1, 1, 1, 0, 0, TimeSpan.FromHours(1))
                });
}