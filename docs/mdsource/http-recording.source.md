# Http Recording

Http Recording allows, when a method is being tested, for any http requests made as part of that method call to be recorded and verified.


## Usage

Call `HttpRecording.StartRecording();` before the method being tested is called.

The perform the verification as usual:

snippet: HttpRecording

The requests/response pairs will be appended to the verified file.

snippet: Tests.TestHttpRecording.verified.txt