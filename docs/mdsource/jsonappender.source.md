# JsonAppender

A JsonAppender allows extra content (key value pairs) to be optionally appended to the output being verified. JsonAppenders can use the current context to determine what should be appended or if anything should be appended.

Register a JsonAppender:

snippet: RegisterJsonAppender

When when content is verified:

snippet: JsonAppender

The content from RegisterJsonAppender will be included in the output:

snippet: JsonAppenderTests.WithJsonAppender.verified.txt


The name part of the JsonAppender can be inferred:

snippet: JsonAppenderInferredName

Results in:

snippet: JsonAppenderTests.WithInferredNameJsonAppender.verified.txt


If the target is a stream or binary file:

snippet: JsonAppenderStream

Then the appended content will be added to the `.verified.txt` file:

snippet: JsonAppenderTests.Stream#00.verified.txt

See [Converters](/docs/converter.md) for more information on `*.00.verified.txt` files.

Examples of extensions using JsonAppenders are [Recorders in Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording) and  [Recorders in Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording).