# VerifyZip

Verifies all files in a zip archive. This approach combines [UseUniqueDirectory](/docs/naming.md#useuniquedirectory) with a target per file, to snapshot test all files in a zip archive.

snippet: VerifyZipXunitV3


## Filtering

snippet: VerifyZipFilterXunitV3


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.

snippet: VerifyZipWithInfoXunitV3


## FileScrubber

`VerifyZip` has an optional parameter `fileScrubber` that allows file specific scrubbing:

snippet: VerifyZipWithFileScrubberXunitV3

This applies to files where the extensions is a known text file as defined by [FileExtensions.IsText](https://github.com/VerifyTests/EmptyFiles#istext).


## Including structure

Use `includeStructure: true` to include a file `structure.verified.md` that contains the zip directory structure.

snippet: VerifyZipWithStructureXunitV3


## PersistArchive

`persistArchive` determines whether the original ZipArchive should be preserved as a verified file.

snippet: WithZipAndPersistArchiveV3