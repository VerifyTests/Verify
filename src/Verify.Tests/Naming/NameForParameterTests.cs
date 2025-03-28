﻿public class NameForParameterTests
{
    [Fact]
    public Task Null() =>
        Verify(VerifierSettings.GetNameForParameter(null, counter: CounterBuilder.Empty()));

    [Fact]
    public Task StringEmpty() =>
        Verify(VerifierSettings.GetNameForParameter("", counter: CounterBuilder.Empty()));

    [Fact]
    public Task StringInvalidPathChar() =>
        Verify(VerifierSettings.GetNameForParameter("a/a", counter: CounterBuilder.Empty()));

    [Fact]
    public Task Int() =>
        Verify(VerifierSettings.GetNameForParameter(10, counter: CounterBuilder.Empty()));

    [Fact]
    public Task NullCounter() =>
        Verify(VerifierSettings.GetNameForParameter(10, counter: null));

#if NET6_0_OR_GREATER
    [Fact]
    public Task Half() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                (Half) 10,
                counter: CounterBuilder.Empty()));
#endif

#if NET6_0_OR_GREATER
    [Fact]
    public Task Date() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                new Date(2000, 10, 1),
                counter: CounterBuilder.Empty()));

    [Fact]
    public Task Time() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                new Date(2000, 10, 1),
                counter: CounterBuilder.Empty()));
#endif

    [Fact]
    public Task DateTimeLocal()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(
            VerifierSettings.GetNameForParameter(
                date,
                counter: CounterBuilder.Empty()));
    }

    [Fact]
    public Task DateTimeUnspecified()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(
            VerifierSettings.GetNameForParameter(
                date,
                counter: CounterBuilder.Empty()));
    }

    [Fact]
    public Task DateTimeUtc()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(
            VerifierSettings.GetNameForParameter(
                date,
                counter: CounterBuilder.Empty()));
    }

    [Fact]
    public Task DateTimeOffsetUtc()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(
            VerifierSettings.GetNameForParameter(
                date,
                counter: CounterBuilder.Empty()));
    }

    [Fact]
    public Task List() =>
        Verify(VerifierSettings.GetNameForParameter(
            new List<string>
            {
                "value"
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task ListMultiple() =>
        Verify(
            VerifierSettings.GetNameForParameter(
            new List<string>
            {
                "value1",
                "value2"
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task Nested() =>
        Verify(VerifierSettings.GetNameForParameter(
            new List<object>
            {
                "value1",
                new Dictionary<string, int>
                {
                    {
                        "value2", 10
                    },
                    {
                        "value3", 20
                    }
                },
                "value4"
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task EmptyList() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                new List<string>(),
                counter: CounterBuilder.Empty()));

    [Fact]
    public Task Dictionary() =>
        Verify(VerifierSettings.GetNameForParameter(
            new Dictionary<string, int>
            {
                {
                    "value", 10
                }
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task DictionaryMultiple() =>
        Verify(VerifierSettings.GetNameForParameter(
            new Dictionary<string, int>
            {
                {
                    "value1", 10
                },
                {
                    "value2", 11
                }
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task EmptyDictionary() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                new Dictionary<string, int>(),
                counter: CounterBuilder.Empty()));

    [Fact]
    public Task EnumerableStaticEmpty() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                Enumerable.Empty<string>(),
                counter: CounterBuilder.Empty()));

    [Fact]
    public Task Array() =>
        Verify(
            VerifierSettings.GetNameForParameter(
            new[]
            {
                "value"
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task ArrayMultiple() =>
        Verify(VerifierSettings.GetNameForParameter(
            new[]
            {
                "value1",
                "value2"
            },
            counter: CounterBuilder.Empty()));

    [Fact]
    public Task ArrayEmpty() =>
        Verify(
            VerifierSettings.GetNameForParameter(
                System.Array.Empty<string>(),
                counter: CounterBuilder.Empty()));
}