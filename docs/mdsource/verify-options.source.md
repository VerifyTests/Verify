# Verify Options


## OnHandlers

 * `OnVerify` takes two actions that are called before and after each verification.
 * `OnFirstVerify` is called when there is no verified file.
 * `OnVerifyMismatch` is called when a received file does not match the existing verified file.


### AutoVerify

OnHandlers are called before [AutoVerify](autoverify.md) logic being applied. So for example in the case of `OnVerifyMismatch`, both the received and verified file will exist at the point `OnVerifyMismatch` is called. Immediately after received will be used to overwrite verified.


### Globally

snippet: OnStaticHandlers


### Instance

snippet: OnInstanceHandlers


### Fluent

snippet: OnFluentHandlers


## OmitContentFromException

By default, when a verify mismatch occurs for text, the content of the received and verified files is included in the exception that is thrown. This results in that text being included in test runners and build output. To omit the content use `VerifierSettings.OmitContentFromException`.


## DisableDiff

To disable diff launching:

snippet: DisableDiff
