# VerifyDirectory

Verified all files in a directory. This approach combines [UseUniqueDirectory](/docs/naming.md#useuniquedirectory) with a target per file, to snapshot test all files in a directory.

snippet: VerifyDirectoryXunit


## Filtering

snippet: VerifyDirectoryFilterXunit


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.