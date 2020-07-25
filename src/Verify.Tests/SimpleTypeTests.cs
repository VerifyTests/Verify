using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SimpleTypeTests
{
    [Theory]
    [MemberData(nameof(GetData))]
    public Task Run(object arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg.GetType());
        return Verifier.Verify(arg, settings);
    }

    [Fact]
    public Task StringWrappedInTask()
    {
        return Verifier.Verify(Task.FromResult("theString"));
    }

    [Fact]
    public Task NullWrappedInTask()
    {
        return Verifier.Verify(Task.FromResult<object?>(null));
    }

    [Fact]
    public Task Null()
    {
        return Verifier.Verify((object?)null);
    }

    [Fact]
    public Task DateTimeWrappedInTask()
    {
        return Verifier.Verify(Task.FromResult(new DateTime(2000, 1, 1, 1, 1, 1, 1)));
    }

    [Fact]
    public Task GuidWrappedInTask()
    {
        return Verifier.Verify(Task.FromResult(new Guid("ebced679-45d3-4653-8791-3d969c4a986c")));
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"theString"};
        yield return new object[] {true};
        yield return new object[] {(long) 1};
        yield return new object[] {(short) 1};
        yield return new object[] {1};
        yield return new object[] {(uint) 1};
        yield return new object[] {(ulong) 1};
        yield return new object[] {(ushort) 1};
        yield return new object[] {(decimal) 1};
        yield return new object[] {(float) 1};
        yield return new object[] {new Guid("ebced679-45d3-4653-8791-3d969c4a986c")};
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<body>
    <node>text</node>
</body>";
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        yield return new object[] {xmlDocument};
        var xDocument = XDocument.Parse(xml);
        yield return new object[] {xDocument};
        yield return new object[] {new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc)};
        yield return new object[] {new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, TimeSpan.FromHours(1))};
    }
}