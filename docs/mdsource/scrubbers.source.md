# Scrubbers

Scrubbers run on the final string prior to doing the verification action.

Multiple scrubbers can bee defined level.

Scrubber are executed in reveres order. So the most recent added method scrubber through to earliest added global scrubber.


## XUnit

snippet: ScrubbersSampleXunit


## NUnit

snippet: ScrubbersSampleNUnit


## MSTest

snippet: ScrubbersSampleMSTest


## Results

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.Lines.verified.txt

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.ScrubberAppliedAfterJsonSerialization.verified.txt


## Scrubber levels

Scrubbers can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: As a class level 'VerifySettings' field then re-used at the method level.
 * Global: Will run for test methods on all tests.

Global scrubbers should be defined only once at appdomain startup. In this example the scrubber is configured using the [Global Setup](https://github.com/SimonCropp/XunitContext#global-setup) of [XunitContext](https://github.com/SimonCropp/XunitContext). It could also be configured using a [Module Initializer](https://github.com/Fody/ModuleInit).


### XUnit

snippet: ScrubberLevelsSampleXunit


### NUnit

snippet: ScrubberLevelsSampleNUnit


### MSTest

snippet: ScrubberLevelsSampleMSTest


### Result

snippet: Verify.Xunit.Tests/Scrubbers/ScrubberLevelsSample.Simple.verified.txt