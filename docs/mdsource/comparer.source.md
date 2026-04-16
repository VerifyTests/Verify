# Comparer

Comparers are used to compare non-text files.


## Custom Comparer

Using a custom comparer can be helpful when a result has changed, but not enough to fail verification. For example when rendering images/forms on different operating systems.

For samples purposes only the image sizes will be compared:

snippet: ImageComparer

The returned `CompareResult.NotEqual` takes an optional message that will be rendered in the resulting text displayed to the user on test failure.

**If an input is split into multiple files, and a text file fails, then all subsequent binary comparisons will revert to the default comparison.**


### Instance comparer

snippet: InstanceComparer


### Static comparer

snippet: StaticComparer


## Default Comparison

snippet: DefualtCompare


## PNG SSIM comparer

Verify includes a built-in [Structural Similarity Index](https://en.wikipedia.org/wiki/Structural_similarity_index_measure) (SSIM) comparer for PNG files. It is opt-in and, when enabled, replaces the default byte-for-byte comparison for the `.png` extension.

This is useful when rendered images differ slightly between runs (e.g. anti-aliasing, font hinting, platform-specific rasterization) but are perceptually identical.

snippet: UseSsimForPng

The default threshold is `0.98`. SSIM scores range from `0` (completely different) to `1` (identical). A custom threshold can be supplied:

snippet: UseSsimForPngThreshold

Dimension mismatches between the received and verified images are always reported as not equal, regardless of threshold.


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