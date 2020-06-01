# Scrubbers

Scrubbers run on the final string before doing the verification action.

Multiple scrubbers [can be defined at multiple levels](#Scrubber-levels).

Scrubber are executed in reveres order. So the most recent added method scrubber through to earliest added global scrubber.


## Available Scrubbers

Scrubbers can be added to an instance of `VerifySettings` or globally on `SharedVerifySettings`.


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

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.ScrubberAppliedAfterJsonSerialization.verified.txt


## Scrubber levels

Scrubbers can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: As a class level 'VerifySettings' field then re-used at the method level.
 * Global: Will run for test methods on all tests.

Global scrubbers should be defined only once at appdomain startup. In this example the scrubber is configured using the [Global Setup](https://github.com/SimonCropp/XunitContext#global-setup) of [XunitContext](https://github.com/SimonCropp/XunitContext). It could also be configured using a [Module Initializer](https://github.com/Fody/ModuleInit).


### xUnit

snippet: ScrubberLevelsSampleXunit


### NUnit

snippet: ScrubberLevelsSampleNUnit


### MSTest

snippet: ScrubberLevelsSampleMSTest


### Result

snippet: Verify.Xunit.Tests/Scrubbers/ScrubberLevelsSample.Usage.verified.txt