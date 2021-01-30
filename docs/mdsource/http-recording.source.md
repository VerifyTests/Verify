# Http Recording

Http Recording allows, when a method is being tested, for any http requests made as part of that method call to be recorded and verified.

**Supported in netstandard2.1 and up**

## Usage

Call `HttpRecording.StartRecording();` before the method being tested is called.

The perform the verification as usual:

snippet: HttpRecording

The requests/response pairs will be appended to the verified file.

snippet: Tests.TestHttpRecording.verified.txt


## Explicit Usage

The above usage results in the http calls being automatically added snapshot file. Calls can also be explicitly read and recorded using `HttpRecording.FinishRecording()`. This enables:

 * Filtering what http calls are included in the snapshot.
 * Only verifying a subset of information for each http call.
 * Performing additional asserts on http calls.

For example:

snippet: HttpRecordingExplicit

Results in the following:

snippet: Tests.TestHttpRecordingExplicit.verified.txt