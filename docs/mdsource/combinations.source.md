# VerifyCombinations

VerifyCombinations allows all combinations of the given input lists to be executed, and the results all written to one file.


## Method being tested

snippet: CombinationTargetMethod


## Test

snippet: CombinationSample


## Result

snippet: VerifyCombinationsSample.BuildAddressTest.verified.txt


## Column Alignment

Key value are aligned based on type.

 * Numbers (int, double, float etc) are aligned right
 * All other types are aligned left


## CaptureExceptions

By default exceptions are not captured.

To enable exception capture use `captureExceptions = true`:

snippet: CombinationSample_CaptureExceptions


### Result

snippet: VerifyCombinationsSample.BuildAddressExceptionsTest.verified.txt


### Global CaptureExceptions

Exception capture can be enable globally:

snippet: GlobalCaptureExceptions

If exception capture has been enabled globally, it can be disable at the method test level using `captureExceptions: false`.

snippet: CombinationSample_CaptureExceptionsFalse


## Result serialization

Serialization of results is done using `CombinationResultsConverter`

snippet: CombinationResultsConverter.cs


### Custom

Combination serialization can be customized using a Converter.


#### Converter

Inherit from `CombinationResultsConverter` and override the desired members.

The below sample override `BuildPropertyName` to customize the property name. It bypasses the default implementation and hence does not pad columns or use VerifierSettings.GetNameForParameter for key conversion.

snippet: CombinationSample_CustomSerializationConverter

Full control of serialization can be achieved by inheriting from `WriteOnlyJsonConverter<CombinationResults>`.


#### Insert Converter

Insert the new converter at the top of the converter stack.

snippet: CombinationSample_CustomSerializationModuleInitializer


#### Result

snippet: VerifyCombinationsTests.Combination_CustomSerialization.verified.txt