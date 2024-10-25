# VerifyCombinations

VerifyCombinations allows all combinations of the given input lists to be executed, and the results all written to one file.


## Method being tested

snippet: CombinationTargetMethod


## Test

snippet: CombinationSample


## Result

snippet: VerifyCombinationsSample.BuildAddressTest.verified.txt


## CaptureExceptions

By default exceptions are not captured.

To enable exception capture use `captureExceptions = true`

snippet: CombinationSample_CaptureExceptions


### Result

snippet: VerifyCombinationsSample.BuildAddressExceptionsTest.verified.txt


### Global CaptureExceptions

Exception capture can be enable globally:

snippet: GlobalCaptureExceptions

If exception capture has been enabled globally, it can be disable at the method test level using `captureExceptions: false`.

snippet: CombinationSample_CaptureExceptionsFalse