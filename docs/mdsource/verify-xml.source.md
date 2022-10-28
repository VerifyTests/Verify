# VerifyXml

Verifies Xml:

 * Scrubbing respected.
 * Pretty prints

snippet: VerifyFile


Give the following Xml:

```
<body><node>text</node></body>
```

This code:

snippet: VerifyXml

Will produce

snippet: XmlTests.Xml.verified.xml


## IgnoreMember

This code:

snippet: XmlIgnoreMember

Will produce

snippet: XmlTests.XmlIgnoreMember.verified.xml


## ScrubMember

This code:

snippet: XmlScrubMember

Will produce

snippet: XmlTests.XmlScrubMember.verified.xml
