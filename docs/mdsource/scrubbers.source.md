# Scrubbers

Scrubbers run on the final string before doing the verification action.

Multiple scrubbers [can be defined at multiple levels](#Scrubber-levels).

Scrubber are executed in reveres order. So the most recent added method scrubber through to earliest added global scrubber.


## Available Scrubbers

Scrubbers can be added to an instance of `VerifySettings` or globally on `VerifierSettings`.


### Directory Scrubbers

 * The current solution directory will be replaced with `{SolutionDirectory}`. To disable use `VerifierSettings.DontScrubSolutionDirectory()` in a module initializer.
 * The current project directory will be replaced with `{ProjectDirectory}`. To disable use `VerifierSettings.DontScrubProjectDirectory()` in a module initializer.
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

Case insensitive by default (StringComparison.OrdinalIgnoreCase).

`StringComparison` can be overridden:

snippet: ScrubLinesContainingOrdinal


### ScrubLinesWithReplace

Allows lines to be selectively replaced using a `Func`.

For example converts lines to upper case:

snippet: ScrubLinesWithReplace


### ScrubMachineName

Replaces `Environment.MachineName` with `TheMachineName`.

snippet: ScrubMachineName


### AddScrubber

Adds a scrubber with full control over the text via a `Func`


## More complete example


### xUnit

snippet: ScrubbersSampleXunit


### NUnit

snippet: ScrubbersSampleNUnit


### MSTest

snippet: ScrubbersSampleMSTest


### Results

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.Lines.verified.txt

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.AfterSerialization.verified.txt


## Scrubber levels

Scrubbers can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: As a class level 'VerifySettings' field then re-used at the method level.
 * Global: Will run for test methods on all tests.


### xUnit

snippet: ScrubberLevelsSampleXunit


### NUnit

snippet: ScrubberLevelsSampleNUnit


### MSTest

snippet: ScrubberLevelsSampleMSTest


### Result

snippet: Verify.Xunit.Tests/Scrubbers/ScrubberLevelsSample.Usage.verified.txt

