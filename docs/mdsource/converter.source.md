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


### Stream Conversion

This sample uses a extension approach. So the converter acts on a file or stream based on the extension (configured or detected).

snippet: RegisterStreamConverterExtension

snippet: FileConverterExtensionVerify


### Text extensions

A stream converter can also be registered against a text extension. This is useful when a text document needs derived targets, for example rendering html to an image for visual verification.

The text target is scrubbed before being passed to the converter, so any derived targets (for example a rendered image) reflect the scrubbed content.

For a custom text extension, register it as text via `FileExtensions.AddTextExtension`. Built-in text extensions (for example `html` or `csv`) do not require this.

snippet: RegisterStreamConverterTextExtension

snippet: TextExtensionConverterVerify

A target for a text extension can also be built from a stream. The stream is read as text, so a converter can re-emit its source without converting it back to a string:

snippet: RegisterStreamConverterTextExtensionStream


### Cleanup

If cleanup needs to occur after verification a callback can be passes to `ConversionResult`:

snippet: ConversionResultWithCleanup


## Excluding targets

Some converters emit the source document (for example a `pdf`, `docx`, or `xlsx`) alongside the info file and the derived targets. That source document is then committed as a `.verified.{extension}` file. Where the document is large, or where its bytes cannot be made deterministic, it can be excluded from the snapshot. The info file and the derived targets continue to verify.

`ExcludeTargets` takes one or more extensions, and drops every matching target:

snippet: ExcludeTargets

Any existing verified file for an excluded extension is then reported as pending deletion.

To exclude an extension for every test, call `ExcludeTargets` on `VerifierSettings` at initialization:

snippet: StaticExcludeTargets

Excluding every target of a verification is an error, since a verification requires at least one target.


### Avoiding work in a converter

Building the source document (for example rendering a pdf or a docx) can be expensive. When a target is excluded, it is dropped after the converter has produced it, so the work is wasted. A converter can instead check `IsTargetExcluded` on its `context` and skip producing the target entirely:

snippet: ConverterExcludeCheck

`IsTargetExcluded` reflects both the global and the per-verification `ExcludeTargets`, so shipping this check lets a caller opt out of the document build itself, rather than only its snapshot.


## Shipping

Converters can be shipped as NuGet packages:

 * [Verify.Aspose](https://github.com/VerifyTests/VerifyTests.Aspose): Verification of documents (pdf, docx, xlsx, and pptx) via Aspose.
 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework): Verification of EntityFramework bits.
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp](https://github.com/VerifyTests/Verify.ImageSharp): Verification of images via [ImageSharp](https://github.com/SixLabors/ImageSharp).
 * [Verify.NServiceBus](https://github.com/NServiceBusExtensions/Verify.NServiceBus): Verify NServiceBus Test Contexts.
 * [Verify.RavenDb](https://github.com/VerifyTests/Verify.RavenDb): Verification of [RavenDb](https://ravendb.net) bits.
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer): Verification of SqlServer bits.
 * [Verify.Syncfusion](https://github.com/VerifyTests/Verify.Syncfusion): Converts documents (pdf, docx, xlsx, and pptx) to png/csv/text for verification.
 * [Verify.Web](https://github.com/VerifyTests/Verify.Web): Verification of web bits.
 * [Verify.WinForms](https://github.com/VerifyTests/Verify.WinForms): Verification of WinForms UIs.
 * [Verify.Xaml](https://github.com/VerifyTests/Verify.Xaml): Verification of Xaml UIs.