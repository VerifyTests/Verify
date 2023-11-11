# Verification of binary data

Recording allows information to be statically captured and then (optionally) verified.


## Usage

snippet: Recording

snippet: RecordingTests.Usage.verified.txt


## Grouping

Values are grouped by key:

snippet: RecordingSameKey

snippet: RecordingTests.SameKey.verified.txt

To avoid grouping use [Stop](#stop).


## Identifier

Recording can be grouped by an identifier.

snippet: RecordingIdentifier

snippet: RecordingTests.Identifier.verified.txt


## Case is ignored

snippet: RecordingIgnoreCase

snippet: RecordingTests.Case.verified.txt


## Stop

Recording can be stopped and the resulting data can be manually verified:

snippet: RecordingStop

snippet: RecordingTests.Stop.verified.txt

If Stop is called, the results are not automatically verified:

snippet: RecordingStopNotInResult

snippet: RecordingTests.StopNotInResult.verified.txt


## IsRecording

The status of Recording can be checked.

snippet: IsRecording

This can be helpful if the cost of capturing data, to add to recording, is high.


## Clear

The current recorded items can be cleared:

snippet: RecordingClear

snippet: RecordingTests.Clear.verified.txt


## Pause and Resume

Recording can be paused and resumed:

snippet: RecordingPauseResume

snippet: RecordingTests.PauseResume.verified.txt


## Extensions that leverage Recording

 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording)
 * [Verify.Http](https://github.com/VerifyTests/Verify.Http)
 * [Verify.MassTransit](https://github.com/VerifyTests/Verify.MassTransit)
 * [Verify.MicrosoftLogging](https://github.com/VerifyTests/Verify.MicrosoftLogging)
 * [Verify.NServiceBus](https://github.com/VerifyTests/Verify.NServiceBus#recording)
 * [Verify.Serilog](https://github.com/VerifyTests/Verify.Serilog)
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording)
 * [Verify.ZeroLog](https://github.com/VerifyTests/Verify.ZeroLog)