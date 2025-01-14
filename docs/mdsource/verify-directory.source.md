# VerifyDirectory

Verifies all files in a directory. This approach combines [UseUniqueDirectory](/docs/naming.md#useuniquedirectory) with a target per file, to snapshot test all files in a directory.

snippet: VerifyDirectoryXunitV3


## Filtering

snippet: VerifyDirectoryFilterXunitV3


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.

snippet: VerifyDirectoryWithInfoXunitV3


## FileScrubber

`VerifyDirectory` has an optional parameter `fileScrubber` that allows file specific scrubbing:

snippet: VerifyDirectoryWithFileScrubberXunitV3

This applies to files where the extensions is a known text file as defined by [FileExtensions.IsText](https://github.com/VerifyTests/EmptyFiles#istext).


## Files with no extension

Any files in the target directory that have no extension will have `.noextension` added to the resulting `.verified` file. This is to avoid `.verified` being treated as the extension of that file.

These files are treated as unknown binaries by default. Given the lack of an extension, no diff tool will be launched.

Files with no extension can optionally be treated as text, with the associated diff tool being launched. This is done by using the [AddTextFileConvention](https://github.com/VerifyTests/EmptyFiles?tab=readme-ov-file#addtextfileconvention) of EmptyFiles:

snippet: AddTextFileConvention