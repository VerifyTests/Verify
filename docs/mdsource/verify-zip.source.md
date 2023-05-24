# VerifyZip

Verifies all files in a zip archive. This approach combines [UseUniqueDirectory](/docs/naming.md#useuniquedirectory) with a target per file, to snapshot test all files in a zip archive.

snippet: VerifyZipXunit


## Filtering

snippet: VerifyZipFilterXunit


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.

snippet: VerifyZipWithInfo


## FileScrubber

`VerifyDirectory` has an optional parameter `fileScrubber` that allows file specific scrubbing:

snippet: VerifyZipWithFileScrubber

This applies to files where the extensins is a known text file as defined by [FileExtensions.IsText](https://github.com/VerifyTests/EmptyFiles#istext).