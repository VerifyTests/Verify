# Scrubbers

Scrubbers run on the final string prior to doing the verification action.

They can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.

Multiple scrubbers can bee defined at each level.

Scrubber are excited in reveres order. So the most recent added method scrubber through to earliest added global scrubber.

Global scrubbers should be defined only once at appdomain startup. In this example the scrubber is configured using the [Global Setup](https://github.com/SimonCropp/XunitContext#global-setup) of [XunitContext](https://github.com/SimonCropp/XunitContext). It could also be configured using a [Module Initializer](https://github.com/Fody/ModuleInit).


## XUnit

snippet: ScrubbersSampleXunit


## NUnit

snippet: ScrubbersSampleNUnit


## MSTest

snippet: ScrubbersSampleMSTest


## Results

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.Simple.verified.txt

snippet: Verify.Xunit.Tests/Scrubbers/ScrubbersSample.AfterJson.verified.txt

