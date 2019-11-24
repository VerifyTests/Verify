# Scrubbers

Scrubbers run on the final string prior to doing the verification action.

They can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.

Multiple scrubbers can bee defined at each level.

Scrubber are excited in reveres order. So the most recent added method scrubber through to earlies added global scrubber.

Global scrubbers should be defined only once at appdomain startup.

Usage:

snippet: scrubberssample.cs

Results:

snippet: ScrubbersSample.Simple.verified.txt

snippet: ScrubbersSample.AfterJson.verified.txt

