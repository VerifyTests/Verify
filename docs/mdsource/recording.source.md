# Recording

Recording allows information to be statically captured and then (optionally) verified.

The main value of this feature is to simplify addition of information to a snapshot from extensions to Verify. Before to this feature, for an extension to supply information to a snapshot, that extension had to return the information up the stack, and the calling test had to explicitly add that information to the `Verify()` call. Using this feature an extension can `Recording.Add()` in any context, and the information will be added to the snapshot.


## Usage

snippet: Recording

Results in:

snippet: RecordingTests.Usage.verified.txt


## TryAdd

If `Recording.Add()` is called before `Recording.Start` an exception will be called.

`Recording.TryAdd()` will add an item only if `Recording.IsRecording` is true.

snippet: RecordingTryAdd


## Scoped

Recording can be scoped via a `using`:

snippet: RecordingScoped

Results in:

snippet: RecordingTests.RecordingScoped.verified.txt


## Grouping

Values are grouped by key:

snippet: RecordingSameKey

Results in:

snippet: RecordingTests.SameKey.verified.txt

To avoid grouping use [Stop](#stop).


## Identifier

Recording can be grouped by an identifier.

snippet: RecordingIdentifier

Results in:

snippet: RecordingTests.Identifier.verified.txt


## Case is ignored

snippet: RecordingIgnoreCase

Results in:

snippet: RecordingTests.Case.verified.txt


## Stop

Recording can be stopped and the resulting data can be manually verified:

snippet: RecordingStop

Results in:

snippet: RecordingTests.Stop.verified.txt

If Stop is called, the results are not automatically verified:

snippet: RecordingStopNotInResult

Results in:

snippet: RecordingTests.StopNotInResult.verified.txt


## IsRecording

The status of Recording can be checked.

snippet: IsRecording

This can be helpful if the cost of capturing data, to add to recording, is high.


## Clear

The current recorded items can be cleared:

snippet: RecordingClear

Results in:

snippet: RecordingTests.Clear.verified.txt


## Pause and Resume

Recording can be paused and resumed:

snippet: RecordingPauseResume

Results in:

snippet: RecordingTests.PauseResume.verified.txt


## Extensions that leverage Recording

 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording)
 * [Verify.Http](https://github.com/VerifyTests/Verify.Http)
 * [Verify.MicrosoftLogging](https://github.com/VerifyTests/Verify.MicrosoftLogging)
 * [Verify.NServiceBus](https://github.com/VerifyTests/Verify.NServiceBus#recording)
 * [Verify.Serilog](https://github.com/VerifyTests/Verify.Serilog)
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording)
 * [Verify.ZeroLog](https://github.com/VerifyTests/Verify.ZeroLog)