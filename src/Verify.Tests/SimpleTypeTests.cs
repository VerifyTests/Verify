[UsesVerify]
public class SimpleTypeTests
{
#if NET5_0_OR_GREATER
    [Theory]
    [MemberData(nameof(GetData))]
    public Task Run(object arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg.GetType());
        return Verify(arg, settings);
    }
#endif

    [Fact]
    public Task StringWrappedInTask() =>
        Verify(Task.FromResult("theString"));

    [Fact]
    public Task StringEmptyWrappedInTask() =>
        Verify(Task.FromResult(string.Empty));

    [Fact]
    public Task StringNullWrappedInTask() =>
        Verify(Task.FromResult((string) null!));

    [Fact]
    public Task StringEmpty() =>
        Verify(string.Empty);

    [Fact]
    public Task StringNull() =>
        Verify((string?) null);

    [Fact]
    public Task Null() =>
        Verify((object?) null);

    [Fact]
    public Task NullWrappedInTask() =>
        Verify(Task.FromResult<int?>(null!));

#if NET5_0_OR_GREATER
    [Fact]
    public Task DateTimeWrappedInTask() =>
        Verify(Task.FromResult(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc)));

#endif

    [Fact]
    public Task GuidWrappedInTask() =>
        Verify(Task.FromResult(new Guid("ebced679-45d3-4653-8791-3d969c4a986c")));

    public static IEnumerable<object[]> GetData()
    {
        yield return [new KeyValuePair<string, int>("theKey", 10)];

        var json = """
                   {
                     'short': {
                       'original': 'http://www.foo.com/',
                       'short': 'foo',
                       'error': {
                         'code': 0,
                         'msg': 'No action taken'
                       }
                     }
                   }
                   """;
        var argonJToken = JToken.Parse(json);
        yield return
        [
            argonJToken
        ];

        yield return
        [
            JArray.Parse(
                """
                [
                  'Small',
                  'Medium',
                  'Large'
                ]
                """)
        ];
        yield return [true];
        yield return ["stringValue"];
        yield return [File.OpenRead("sample.png")];
        yield return
        [
            new byte[]
            {
                1
            }
        ];
        yield return [(long) 1];
        yield return [(short) 1];
        yield return [1];
        yield return [(uint) 1];
        yield return [(ulong) 1];
        yield return [(ushort) 1];
        yield return [(decimal) 1.1];
        yield return [(float) 1.1];
        yield return [new Guid("ebced679-45d3-4653-8791-3d969c4a986c")];
        yield return [new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc).ToUniversalTime()];
        yield return [new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, TimeSpan.FromHours(1)).ToUniversalTime()];
#if NET6_0_OR_GREATER
        yield return [(Half) 10];
        yield return [new Date(2000, 1, 1)];
        yield return [new Time(1, 1)];
#endif
    }
}