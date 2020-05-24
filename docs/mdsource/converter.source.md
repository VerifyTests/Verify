# Converters

Converters are used to split a target into its component parts, then verify each of those parts.

When a target is split the result the result is:

 * An info file (containing the metadata of the target) serialized as json. File name: `{TestType}.{TestMethod}.info.verified.txt`
 * Zero or more documents of a specified extension. File name: `{TestType}.{TestMethod}.{Index}.verified.{Extension}`


## Usage scenarios

 * tiff => png per page
 * spreadsheet => csv per sheet
 * pdf => pdf per page


## Example

Both the following examples take an input tiff and convert it to:

The info file:

snippet: ConverterSnippets.Type.info.verified.txt

Multiple png files:

<img src="../src/Verify.Tests/Snippets/ConverterSnippets.Type.00.verified.png" alt="Converter page one verified" width="200">
<img src="../src/Verify.Tests/Snippets/ConverterSnippets.Type.01.verified.png" alt="Converter page one verified" width="200">


### Typed converter

This sample uses a typed approach. So the converter acts on an in memory instance matching based on type.

snippet: RegisterFileConverterType

snippet: FileConverterTypeVerify

Note that this sample also uses the optional `canConvert` to ensure that only `Image`s that are tiffs are converted.

snippet: ConverterCanConvert


### Expression converter

This sample uses a extension approach. So the converter acts on a file or stream based on the extension (configured or detected).

snippet: RegisterFileConverterExtension

snippet: FileConverterExtensionVerify