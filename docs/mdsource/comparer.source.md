# Comparer

Comparers control how a received result is compared to its verified file. They apply to both text and binary targets.

A comparer is helpful when a result has changed, but not enough to fail verification. For example when rendering images/forms on different operating systems, or when part of a text snapshot legitimately varies between environments.

There are two kinds, selected by the type of the target being compared:

 * String comparers (`StringCompare`) for text targets.
 * Stream comparers (`StreamCompare`) for binary targets.

Note that a comparer only runs when the received and verified content are not already identical. A matching result never reaches the comparer, so the passing path keeps the fast comparison.

In both cases the returned `CompareResult.NotEqual` takes an optional message that will be rendered in the resulting text displayed to the user on test failure.

Comparers change only the equality check. The received and verified content is left as-is, so relaxing a comparison does not churn existing verified files, unlike scrubbing or ignoring members.


## Comparer resolution

Comparers are matched on the extension of the target being compared, and the first match wins:

 1. An instance comparer registered for that extension: `settings.UseStringComparer(compare, "json")`.
 2. An instance comparer registered with no extension, which applies to all extensions: `settings.UseStringComparer(compare)`.
 3. A static comparer registered for that extension: `VerifierSettings.RegisterStringComparer("json", compare)`.
 4. For text targets only, the static default: `VerifierSettings.SetDefaultStringComparer(compare)`.

So a global comparer can be registered once and overridden for specific tests.

The extension match is exact, and an extension with no match is a silent no-op rather than an error. Care is needed where a setting changes the extension of a snapshot: [UseStrictJson](/docs/serializer-settings.md#usestrictjson) renames text snapshots from `txt` to `json`, so a comparer registered against `txt` stops running once it is enabled. `SetDefaultStringComparer` avoids pinning an extension.

Static comparers must be registered before the first verification runs, so a [ModuleInitializer](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.moduleinitializerattribute) is the usual location.


## String comparer

For sample purposes only case differences will be ignored:

snippet: StringComparer


### Instance string comparer

snippet: StringComparerInstance


### Static string comparer

snippet: StringComparerStatic

Or as the default for all text extensions:

snippet: DefaultStringComparer

Information can be passed from a test to its comparer via [Context](/docs/context.md).


## Stream comparer

For samples purposes only the image sizes will be compared:

snippet: ImageComparer

**If an input is split into multiple files, and a text file fails, then all subsequent binary comparisons will revert to the default comparison.**


### Instance stream comparer

snippet: InstanceComparer


### Static stream comparer

snippet: StaticComparer


### Bypass comparers for derived targets

When a converter splits an input into multiple targets, for example a source document plus derived outputs such as rendered images or extracted text, a lenient comparer on a derived target can mask a real change in the source. Setting `BypassComparersForSubsequentOnDifference` on the source target ensures that, when the source differs from its verified file, all subsequent targets skip their registered comparers and fall back to exact comparison:

snippet: BypassComparersForSubsequentOnDifference

The flag must be set on the source target, and that target must precede the derived targets in the conversion result.


## Default Comparison

snippet: DefualtCompare


## PNG SSIM comparer

Verify includes a built-in [Structural Similarity Index](https://en.wikipedia.org/wiki/Structural_similarity_index_measure) (SSIM) comparer for PNG files. It is opt-in and, when enabled, replaces the default byte-for-byte comparison for the `.png` extension.

This is useful when rendered images differ slightly between runs (e.g. anti-aliasing, font hinting, platform-specific rasterization) but are perceptually identical.

snippet: UseSsimForPng

The default threshold is `0.98`. SSIM scores range from `0` (completely different) to `1` (identical). A custom threshold can be supplied:

snippet: UseSsimForPngThreshold

SSIM can also be enabled per-verification, via an instance of `VerifySettings` or fluently on a `SettingsTask`:

snippet: InstanceSsimForPng

Dimension mismatches between the received and verified images are always reported as not equal, regardless of threshold.


### Standalone SSIM scoring

The SSIM score can also be computed directly, outside of a verification. This is useful, for example, to report a similarity metric per rendered page in a custom report:

snippet: SsimCompare

`Ssim.Compare` accepts two PNG streams, two PNG byte arrays, or two `PngImage` instances decoded via `PngDecoder.Decode`. Scores range from `0` (completely different) to `1` (identical). Comparing images of different dimensions throws an `ArgumentException`.

To treat a dimension mismatch as a distinct state instead of an exception, decode with `PngDecoder.Decode` and check dimensions before comparing:

snippet: SsimCompareDimensions

`PngDecoder` supports the same PNG subset as the comparer (see below).


### Supported PNG variants

The bundled decoder targets the common subset of PNGs produced by test scenarios:

 * 8-bit bit depth
 * Color types: grayscale, RGB, RGBA, grayscale+alpha, and paletted (with optional `tRNS` transparency)
 * Non-interlaced images

Unsupported variants (16-bit, Adam7 interlacing) cause the decoder to throw. For scenarios that require full PNG support, use one of the below comparers.


## Pre-packaged comparers

 * [Verify.AngleSharp.Diffing](https://github.com/VerifyTests/Verify.AngleSharp.Diffing): Comparison of html files via [AngleSharp.Diffing](https://github.com/AngleSharp/AngleSharp.Diffing).
 * [Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex): Comparison of text via [DiffPlex](https://github.com/mmanela/diffplex).
 * [Verify.ICSharpCode.Decompiler](https://github.com/VerifyTests/Verify.ICSharpCode.Decompiler): Comparison of assemblies and types via [ICSharpCode.Decompiler](https://github.com/icsharpcode/ILSpy/wiki/Getting-Started-With-ICSharpCode.Decompiler).
 * [Verify.ImageHash](https://github.com/VerifyTests/Verify.ImageHash): Comparison of images via [ImageHash](https://github.com/coenm/ImageHash).
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp.Compare](https://github.com/VerifyTests/Verify.ImageSharp.Compare): Verification and comparison of images via [Codeuctivity.ImageSharp.Compare](https://github.com/Codeuctivity/ImageSharp.Compare).
 * [Verify.Phash](https://github.com/VerifyTests/Verify.Phash): Comparison of images via [Phash](https://github.com/pgrho/phash).
 * [Verify.Quibble](https://github.com/VerifyTests/Verify.Quibble): Comparison of objects via [Quibble](https://github.com/nrkno/Quibble).
 * [YellowDogMan.Verify.ssimulacra2](https://github.com/Yellow-Dog-Man/Verify.ssimulacra2): Verification and comparison of images via [ssimulacra2](https://github.com/cloudinary/ssimulacra2).