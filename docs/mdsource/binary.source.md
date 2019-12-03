# Verification of binary data

Binary data can be verified by passing a stream to `VerifyBinary`.

snippet: VerifyBinary

A [Diff Tool](diff-tool.md) will only be displayed if one can be found that supports the defined extension.

For example if Beyond Compare is detected the following will be displayed:

![Image Diff](image-diff-result.png)

The majority of diff tools require two files to render a diff. When doing the initial verification there is no ".verified." file available. As such when doing the initial verification an "empty file", of the specified extension, will be used. The list of supported empty file can be seen at [EmptyFiles](src/Verify.Xunit/EmptyFiles). If no empty file can be found for a given extension, then no diff will be displayed.