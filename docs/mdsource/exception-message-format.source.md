# Exception Message Format

When a Verify test fails, the exception message is designed to be machine-parsable. This enables tooling (IDE extensions, CI integrations, diff tools) to programmatically extract file paths and take action on verification failures.


## Message Structure

The message has two parts: a **file listing** followed by optional **file content**.


### File Listing

The first line is always the directory:

```
Directory: /path/to/test/project
```

Then zero or more categorized sections, each listing file pairs:

 * **New** - a `.received.` file exists with no corresponding `.verified.` file (first test run or new test).
 * **NotEqual** - both files exist but content differs.
 * **Delete** - a `.verified.` file exists that is no longer produced by any test.
 * **Equal** - both files exist and match (included for completeness when other categories are present).


### File Content

After the file listing, a `FileContent:` section includes the text content of new and not-equal files. This section is only present when there are text-based new or not-equal files. Binary files are listed in the file listing but their content is not included.


## Example: All Categories

snippet: ExceptionMessageFormatSamples.AllCategories.verified.txt


### NotEqual with Comparer Message

When a custom comparer provides a message, the content section uses `Compare Result:` instead of inline file content:

snippet: ExceptionMessageFormatSamples.NotEqualWithMessage.verified.txt


### Content Omission

The `FileContent:` section can be suppressed globally:

snippet: OmitContentFromException

This is useful in CI environments where only the file paths are needed.


## Parsing Exception Messages

The [Verify.ExceptionParsing](https://nuget.org/packages/Verify.ExceptionParsing) NuGet package provides a parser for this format.


### Usage

snippet: ExceptionParsing

The `Result` contains:

 * `New` - list of `FilePair` (received and verified paths).
 * `NotEqual` - list of `FilePair`.
 * `Delete` - list of file paths.
 * `Equal` - list of `FilePair`.


### Test Framework Prefixes

Different test frameworks prepend different prefixes to the exception message. The parser handles these automatically:

 * **NUnit**: `VerifyException : Directory: ...`
 * **MSTest**: `Test method ... threw exception:\nVerifyException: Directory: ...`
 * **Other frameworks**: `Directory: ...`
