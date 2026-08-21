# Verification of binary data

Binary data can be verified by passing a stream.

To know how to treat the stream, an extension needs to be provided:

snippet: StreamWithExtension

For a `FileStream` the extension is not required:

snippet: FileStream


## DiffTool

A [Diff Tool](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md) will only be displayed if one can be found that supports the defined extension.

For example if Beyond Compare is detected the following will be displayed:

<img src="image-diff-result.png" alt="Image Diff" width="400">


### The `bin` extension

`Verify(byte[])`, when called without an explicit extension, defaults to the `bin` extension. Most diff tools do not register `bin`, so in that case no diff tool will launch. If the bytes are better rendered as text, pass a text extension instead:

```cs
await Verify(bytes, extension: "txt");
```

Note that a text extension causes the content to be compared as text, with line endings normalized. So it will not assert byte level details such as the presence of a byte order mark. Where those details are the subject of the test, `bin` remains the correct extension, and the absence of a diff tool is expected.

A tool can also be registered for `bin`, or for any other extension, via [DiffTools.AddTool](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.custom.md).


## Initial diff

The majority of diff tools require two files to render a diff. When doing the initial verification there is no ".verified." file available. As such when doing the initial verification an "empty file", of the specified extension, will be used. The list of supported empty file can be seen at [EmptyFiles](/src/Verify.Xunit/EmptyFiles). If no empty file can be found for a given extension, then no diff will be displayed.