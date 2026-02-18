using System.Xml;
using System.Xml.Linq;

public class XmlTests
{
    string xml = """
                 <?xml version="1.0" encoding="UTF-8"?>
                 <body>
                   <node>text</node>
                 </body>
                 """;

    #region VerifyXml

    [Fact]
    public Task Xml() =>
        VerifyXml(xml);

    #endregion

    [Fact]
    public Task NoDeclaration() =>
        VerifyXml(
            """
            <body>
              <node>text</node>
            </body>
            """);

    [Fact]
    public Task Comment() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <person><!-- name is John Doe --></person>
            """);

    [Fact]
    public Task Comment_WithScrub() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <person><!-- value --></person>
            """);
    [Fact]
    public Task Comment_Mix() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <person><!-- name is John Doe -->value</person>
            """);
    [Fact]
    public Task Comment_Mix_WithScrub() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <person><!-- value -->value</person>
            """)
            .AddScrubber(_ => _.Replace("value", "replaced"));

    [Fact]
    public Task CData() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
              <person><![CDATA[name is John Doe]]></person>
            """);

    [Fact]
    public Task CData_WithScrub() =>
        VerifyXml(
                """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <person><![CDATA[value]]></person>
                """)
            .AddScrubber(_ => _.Replace("value", "replaced"));

    [Fact]
    public Task CDataMix() =>
        VerifyXml(
            """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <person><![CDATA[name is John Doe]]>value</person>
            """);

    [Fact]
    public Task CDataMix_WithScrub() =>
        VerifyXml(
                """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <person><![CDATA[value]]>value</person>
                """)
            .AddScrubber(_ => _.Replace("value", "replaced"));

    #region XmlIgnoreMember

    [Fact]
    public Task XmlIgnoreMember() =>
        VerifyXml(xml)
            .IgnoreMember("node");

    #endregion

    #region XmlScrubMember

    [Fact]
    public Task XmlScrubMember() =>
        VerifyXml(xml)
            .ScrubMember("node");

    #endregion

    [Fact]
    public Task XmlDoc()
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return Verify(document);
    }

    [Fact]
    public Task XmlElement()
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return Verify(document.DocumentElement);
    }

    [Fact]
    public Task XmlDocIgnoreMember()
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return Verify(document)
            .IgnoreMember("node");
    }

    [Fact]
    public Task XmlDocScrubMember()
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return Verify(document)
            .ScrubMember("node");
    }

    [Fact]
    public Task XDoc() =>
        Verify(XDocument.Parse(xml));

    [Fact]
    public Task XElement() =>
        Verify(XDocument.Parse(xml)
            .Root);

    [Fact]
    public Task Scrubbing()
    {
        var date = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK");
        var document = XDocument.Parse(
            $"""
             <?xml version="1.0" encoding="UTF-8"?>
             <body>
               <node att="{date}">{date}</node>
             </body>
             """);
        return Verify(document);
    }

    [Fact]
    public Task IgnoreAttribute()
    {
        var document = XDocument.Parse(
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <body>
              <node att="value"/>
            </body>
            """);
        return Verify(document)
            .IgnoreMember("att");
    }

    [Fact]
    public Task ScrubAttribute()
    {
        var document = XDocument.Parse(
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <body>
              <node att="value"/>
            </body>
            """);
        return Verify(document)
            .ScrubMember("att");
    }

    [Fact]
    public Task XDocIgnoreMember() =>
        Verify(XDocument.Parse(xml))
            .IgnoreMember("node");

    [Fact]
    public Task XDocScrubMember() =>
        Verify(XDocument.Parse(xml))
            .ScrubMember("node");

    [Fact]
    public Task DateOnlyScrubbing()
    {
        var date = DateTime.Now.ToString("d");
        return VerifyXml($"<root><date>{date}</date></root>");
    }

    [Fact]
    public Task DateOnlyScrubbingDisableCounting()
    {
        var date = DateTime.Now.ToString("d");
        return VerifyXml($"<root><date>{date}</date></root>")
            .DisableDateCounting();
    }

    [Fact]
    public Task DateOnlyScrubbingAttribute()
    {
        var date = DateTime.Now.ToString("d");
        return VerifyXml($"""<node att="{date}">text</node>""");
    }

    [Fact]
    public Task DateOnlyScrubbingMultiple()
    {
        var date1 = new DateTime(2024, 1, 15).ToString("d");
        var date2 = new DateTime(2024, 6, 20).ToString("d");
        return VerifyXml($"<root><a>{date1}</a><b>{date2}</b><c>{date1}</c></root>");
    }

    [Fact]
    public Task EmptyTag() =>
        VerifyXml(
            """
            <body>
              <empty />
              <node>text</node>
            </body>
            """);

    [Fact]
    public Task EmptyTagWithAttributes() =>
        VerifyXml(
            $"""
             <body>
               <empty id="{Guid.NewGuid()}" att="asdf" />
               <node>text</node>
             </body>
             """);
}
