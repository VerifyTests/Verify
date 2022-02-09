using System.Xml;
using System.Xml.Linq;

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
    public Task StringWrappedInTask()
    {
        return Verify(Task.FromResult("theString"));
    }

    [Fact]
    public Task NullWrappedInTask()
    {
        return Verify(Task.FromResult<object?>(null));
    }

    [Fact]
    public Task StringEmptyWrappedInTask()
    {
        return Verify(Task.FromResult<object?>(string.Empty));
    }

    [Fact]
    public Task Null()
    {
        return Verify((string)null!);
    }

    [Fact]
    public Task StringEmpty()
    {
        return Verify(string.Empty);
    }

#if NET5_0_OR_GREATER

    [Fact]
    public Task DateTimeWrappedInTask()
    {
        return Verify(Task.FromResult(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc)));
    }

#endif

    [Fact]
    public Task GuidWrappedInTask()
    {
        return Verify(Task.FromResult(new Guid("ebced679-45d3-4653-8791-3d969c4a986c")));
    }

    public static IEnumerable<object[]> GetData()
    {
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><body><node>text</node></body>";
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        yield return new object[] {xmlDocument};

        var xDocument = XDocument.Parse(xml);

        yield return new object[] {xDocument};

        yield return new object[] {xDocument.Root!};

        var json = @"{
  'short': {
    'original': 'http://www.foo.com/',
    'short': 'foo',
    'error': {
      'code': 0,
      'msg': 'No action taken'
    }
  }
}";
        var jToken = JToken.Parse(json);

        yield return new object[] {jToken};

        yield return new object[] {"theString"};
        yield return new object[] {true};
        yield return new object[] {(long) 1};
        yield return new object[] {(short) 1};
        yield return new object[] {1};
        yield return new object[] {(uint) 1};
        yield return new object[] {(ulong) 1};
        yield return new object[] {(ushort) 1};
        yield return new object[] {(decimal) 1.1};
        yield return new object[] {(float) 1.1};
        yield return new object[] {new Guid("ebced679-45d3-4653-8791-3d969c4a986c")};
        yield return new object[] {new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc).ToUniversalTime()};
        yield return new object[] {new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, TimeSpan.FromHours(1)).ToUniversalTime()};
        #if NET6_0_OR_GREATER
        yield return new object[] {(Half)10};
        yield return new object[] {new DateOnly(2000, 1, 1)};
        yield return new object[] {new TimeOnly(1, 1)};
        #endif
    }
}