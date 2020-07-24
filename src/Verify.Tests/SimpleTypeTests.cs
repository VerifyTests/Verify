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

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"theString"};
        yield return new object[] {Task.FromResult("theString")};
        yield return new object[] {true};
        yield return new object[] {(long) 1};
        yield return new object[] {(short) 1};
        yield return new object[] {(int) 1};
        yield return new object[] {(uint) 1};
        yield return new object[] {(ulong) 1};
        yield return new object[] {(ushort) 1};
        yield return new object[] {(decimal) 1};
        yield return new object[] {(float) 1};
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<body>
    <node>text</node>
</body>";
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        yield return new object[] {xmlDocument};
        var xDocument = XDocument.Parse(xml);
        yield return new object[] {xDocument};
        yield return new object[] {new DateTime(2000, 1, 1, 1, 1, 1, 1)};
        yield return new object[] {new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, TimeSpan.Zero)};
    }
}