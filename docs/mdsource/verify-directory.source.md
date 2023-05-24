# VerifyDirectory

Verifies all files in a directory. This approach combines [UseUniqueDirectory](/docs/naming.md#useuniquedirectory) with a target per file, to snapshot test all files in a directory.

snippet: VerifyDirectoryXunit


## Filtering

snippet: VerifyDirectoryFilterXunit


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.

snippet: VerifyDirectoryWithInfo


## FileScrubber

`VerifyDirectory` has an optional parameter `fileScrubber` that allows file specific scrubbing:

snippet: VerifyDirectoryWithFileScrubber

This applies to files where the extensins is a known text file as defined by [FileExtensions.IsText](https://github.com/VerifyTests/EmptyFiles#istext).