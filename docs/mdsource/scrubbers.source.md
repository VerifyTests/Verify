# Scrubbers

Scrubbers run on the final string before doing the verification action.

Multiple scrubbers [can be defined at multiple levels](#Scrubber-levels).

Scrubbing is performed by two mechanisms:

 * **The scrub engine**: a span based engine that executes `Scrubber` definitions. All built-in scrubbing (`ScrubLinesContaining`, `ScrubInlineGuids`, `ScrubMachineName`, etc) runs on the engine.
 * **Legacy scrubbers**: `AddScrubber(Action<StringBuilder>)` overloads. These run after the engine, and only when at least one is registered.


## The scrub engine

A `Scrubber` is created via static factory methods and registered via `AddScrubber` at any [level](#Scrubber-levels):

 * `Scrubber.Replace(find, replacement)`: replace every occurrence of a string. Supports a `StringComparison` and an optional word boundary requirement. A multi-pair overload replaces the longest matching find at any position.
 * `Scrubber.Window(minLength, maxLength, matcher)`: slide a window over the text; the matcher returns a replacement or null. Used by the inline guid and date scrubbers.
 * `Scrubber.Match(matcher, minLength, maxLength)`: custom search logic. The matcher locates the next match within a segment.
 * `Scrubber.RemoveLinesContaining(...)`, `Scrubber.RemoveLines(...)`, `Scrubber.ReplaceLines(...)`, `Scrubber.RemoveEmptyLines()`: line scoped scrubbers.

snippet: AddScrubberEngine

`ScrubLines` and `ScrubLinesWithReplace` (and the corresponding `Scrubber` factories) also accept span based delegates (`LineMatch` / `LineReplace`) that avoid allocating a string per line. Use an explicitly typed lambda parameter to select them, e.g. `ScrubLines((CharSpan line) => ...)`; untyped lambdas bind the string overloads.

Engine semantics:

 * **Quarantine**: text produced by a replacement is never re-examined by other engine scrubbers. Legacy scrubbers run afterwards and can still modify it.
 * **Ordering** is engine determined, not registration determined: line removals run first, then line transforms (registration order), then inline scrubbers (unknown max length first, then longest max length first, ties broken by level then registration order). Directory replacements always run last, so scrubbers always see raw paths.
 * **Length skip**: text shorter than a scrubber's minimum match length is never scanned by that scrubber.
 * **Single line rule**: a match may never contain a line break.
 * Text is newline normalized (`\r\n` and `\r` become `\n`) before scrubbers run.

`ScrubberLocation` on the built-in scrub methods is obsolete and ignored; it still applies to legacy `AddScrubber` overloads (default `First` executes in reverse registration order, `Last` in registration order).


## Legacy scrubbers

Instead of being executed by the engine, `AddScrubber(Action<StringBuilder>)` mutates the full text directly. Overloads also accept a `Counter`, and the context dictionary:

snippet: AddScrubber

Since these run after every engine scrubber, they can also modify text that the engine has already replaced.


## Available Scrubbers

Scrubbers can be added to an instance of `VerifySettings` or globally on `VerifierSettings`.


### Directory Scrubbers

 * The current solution directory will be replaced with `{SolutionDirectory}`. To disable use `VerifierSettings.DontScrubSolutionDirectory()` in a module initializer. See [solution Discovery](solution-discovery.md)
 * The current project directory will be replaced with `{ProjectDirectory}`. To disable use `VerifierSettings.DontScrubProjectDirectory()` in a module initializer.
 * On Windows, the current [user profile](https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder) will be replaced with `{UserProfile}`. To disable use `VerifierSettings.DontScrubUserProfile()` in a module initializer.
 * The `AppDomain.CurrentDomain.BaseDirectory` will be replaced with `{CurrentDirectory}`.
 * The `Assembly.CodeBase` will be replaced with `{CurrentDirectory}`.
 * The `Path.GetTempPath()` will be replaced with `{TempPath}`.


#### Attribute data

The solution and project directory replacement functionality is achieved by adding attributes to the target assembly at compile time. For any project that references Verify, the following attributes will be added:

```
[assembly: AssemblyMetadata("Verify.ProjectDirectory", "C:\Code\TheSolution\Project\")]
[assembly: AssemblyMetadata("Verify.SolutionDirectory", "C:\Code\TheSolution\")]
```

This information can be useful to consumers when writing tests, so it is exposed via `AttributeReader`:

 * Project directory for an assembly: `AttributeReader.GetProjectDirectory(assembly)`
 * Project directory for the current executing assembly: `AttributeReader.GetProjectDirectory()`
 * Solution directory for an assembly: `AttributeReader.GetSolutionDirectory(assembly)`
 * Solution directory for the current executing assembly: `AttributeReader.GetSolutionDirectory()`


### ScrubLines

Allows lines to be selectively removed using a `Func`.

For example remove lines containing `text`:

snippet: ScrubLines


### ScrubLinesContaining

Remove all lines containing any of the defined strings.

For example remove lines containing `text1` or `text2`

snippet: ScrubLinesContaining

Case insensitive by default (`StringComparison.OrdinalIgnoreCase`).

`StringComparison` can be overridden:

snippet: ScrubLinesContainingOrdinal


### ScrubLinesWithReplace

Allows lines to be selectively replaced using a `Func`.

For example converts lines to upper case:

snippet: ScrubLinesWithReplace


### ScrubMachineName

Replaces `Environment.MachineName` with `TheMachineName`.

snippet: ScrubMachineName


### ScrubUserName

Replaces `Environment.UserName` with `TheUserName`.

snippet: ScrubUserName


### AddScrubber

Adds a scrubber with full control over the text via a `Func`


## DisableScrubbers

Given the following target

snippet: DisableScrubbersTarget

When scrubbers are disabled the result will be:

snippet: DisableScrubbersTests/Tests.Instance.verified.txt


### Instance

snippet: DisableScrubbers


### Fluent

snippet: DisableScrubbersFluent


## More complete example


### NUnit

snippet: ScrubbersSampleNUnit


### xUnit

snippet: ScrubbersSampleXunit


### Fixie

snippet: ScrubbersSampleFixie


### MSTest

snippet: ScrubbersSampleMSTest


### TUnit

snippet: ScrubbersSampleTUnit


### Results

snippet: Verify.XunitV3.Tests/Scrubbers/ScrubbersSample.Lines.verified.txt


## Extension specific scrubbers

Scrubbers can be scoped to verified files with a matching extension by passing the extension as the first argument. The extension is specified without a leading dot:

snippet: AddScrubberEngineExtension

A scrubber registered this way runs only for verified files with that extension, while a scrubber registered without an extension runs for all of them. Extension scoping is available at every [level](#Scrubber-levels), and for the legacy `AddScrubber(Action<StringBuilder>)` overloads.


## Scrubber levels

Scrubbers can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: As a class level 'VerifySettings' field then re-used at the method level.
 * Global: Will run for test methods on all tests.


### NUnit

snippet: ScrubberLevelsSampleNUnit


### xUnit

snippet: ScrubberLevelsSampleXunit


### Fixie

snippet: ScrubberLevelsSampleFixie


### MSTest

snippet: ScrubberLevelsSampleMSTest


### TUnit

snippet: ScrubberLevelsSampleTUnit


### Result

snippet: Verify.XunitV3.Tests/Scrubbers/ScrubberLevelsSample.Usage.verified.txt


## See also

 * [Guid behavior](guids.md)
 * [Date behavior](dates.md)
 * [Numeric Ids](numeric-ids.md)
 * [Solution Discovery](solution-discovery.md)
