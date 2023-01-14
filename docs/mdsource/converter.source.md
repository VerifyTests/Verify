# Converters

Converters are used to split a target into its component parts, then verify each of those parts.

When a target is split the result is:

 * An info file (containing the metadata of the target) serialized as json. File name: `{TestType}.{TestMethod}.info.verified.txt`
 * Zero or more documents of a specified extension. File name: `{TestType}.{TestMethod}.{Index}.verified.{Extension}`


## Usage scenarios

 * tiff => png per page
 * spreadsheet => csv per sheet
 * pdf => png per page


## Example

Both the following examples take an input tiff and convert it to:

The info file:

snippet: ConverterSnippets.Type.verified.txt

Multiple png files:

<img src="/src/Verify.Tests/Snippets/ConverterSnippets.Type%2300.verified.png" alt="Converter page one verified" width="200"><img src="/src/Verify.Tests/Snippets/ConverterSnippets.Type%2301.verified.png" alt="Converter page one verified" width="200">


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


### Cleanup

If cleanup needs to occur after verification a callback can be passes to `ConversionResult`:

snippet: ConversionResultWithCleanup


## Shipping

Converters can be shipped as NuGet packages:

 * [Verify.Aspose](https://github.com/VerifyTests/VerifyTests.Aspose): Verification of documents (pdf, docx, xslx, and pptx) via Aspose.
 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework): Verification of EntityFramework bits.
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp](https://github.com/VerifyTests/Verify.ImageSharp): Verification of images via [ImageSharp](https://github.com/SixLabors/ImageSharp).
 * [Verify.NServiceBus](https://github.com/NServiceBusExtensions/Verify.NServiceBus): Verify NServiceBus Test Contexts.
 * [Verify.RavenDb](https://github.com/VerifyTests/Verify.RavenDb): Verification of [RavenDb](https://ravendb.net) bits.
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer): Verification of SqlServer bits.
 * [Verify.Web](https://github.com/VerifyTests/Verify.Web): Verification of web bits.
 * [Verify.WinForms](https://github.com/VerifyTests/Verify.WinForms): Verification of WinForms UIs.
 * [Verify.Xaml](https://github.com/VerifyTests/Verify.Xaml): Verification of Xaml UIs.