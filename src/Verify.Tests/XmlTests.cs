using System.Xml;
using System.Xml.Linq;

[UsesVerify]
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
            .IgnoreMember("att");
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