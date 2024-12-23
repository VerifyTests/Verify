<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/jsonappender.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# JsonAppender

A JsonAppender allows extra content (key value pairs) to be optionally appended to the output being verified. JsonAppenders can use the current context to determine what should be appended or if anything should be appended.

Register a JsonAppender:

<!-- snippet: RegisterJsonAppender -->
<a id='snippet-RegisterJsonAppender'></a>
```cs
VerifierSettings.RegisterJsonAppender(
    context =>
    {
        if (ShouldInclude(context))
        {
            return new ToAppend("theData", "theValue");
        }

        return null;
    });
```
<sup><a href='/src/Verify.Tests/Converters/JsonAppenderTests.cs#L7-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-RegisterJsonAppender' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When when content is verified:

<!-- snippet: JsonAppender -->
<a id='snippet-JsonAppender'></a>
```cs
[Fact]
public Task WithJsonAppender() =>
    Verify("TheValue");
```
<sup><a href='/src/Verify.Tests/Converters/JsonAppenderTests.cs#L30-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-JsonAppender' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The content from RegisterJsonAppender will be included in the output:

<!-- snippet: JsonAppenderTests.WithJsonAppender.verified.txt -->
<a id='snippet-JsonAppenderTests.WithJsonAppender.verified.txt'></a>
```txt
{
  target: TheValue,
  theData: theValue
}
```
<sup><a href='/src/Verify.Tests/Converters/JsonAppenderTests.WithJsonAppender.verified.txt#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-JsonAppenderTests.WithJsonAppender.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If the target is a stream or binary file:

<!-- snippet: JsonAppenderStream -->
<a id='snippet-JsonAppenderStream'></a>
```cs
[Fact]
public Task Stream() =>
    Verify(IoHelpers.OpenRead("sample.txt"));
```
<sup><a href='/src/Verify.Tests/Converters/JsonAppenderTests.cs#L64-L70' title='Snippet source file'>snippet source</a> | <a href='#snippet-JsonAppenderStream' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Then the appended content will be added to the `.verified.txt` file:

<!-- snippet: JsonAppenderTests.Stream#00.verified.txt -->
<a id='snippet-JsonAppenderTests.Stream#00.verified.txt'></a>
```txt
{
  target: null,
  theData: theValue
}
```
<sup><a href='/src/Verify.Tests/Converters/JsonAppenderTests.Stream#00.verified.txt#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-JsonAppenderTests.Stream#00.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See [Converters](/docs/converter.md) for more information on `*.00.verified.txt` files.

Examples of extensions using JsonAppenders are [Recorders in Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording) and  [Recorders in Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording).
