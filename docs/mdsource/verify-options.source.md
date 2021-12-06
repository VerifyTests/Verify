# Verify Options


## AutoVerify

In some scenarios it makes sense to auto-accept any changes as part of a given test run. For example:

 * Keeping a text representation of a Database schema in a `.verified.sql` file (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).

This can be done using `AutoVerify()`:

snippet: AutoVerify

Note that auto accepted changes in `.verified.` files remain visible in source control tooling.


## OnHandlers

 * `OnVerify` takes two actions that are called before and after each verification.
 * `OnFirstVerify` is called when there is no verified file.
 * `OnVerifyMismatch` is called when a received file does not match the existing verified file.

snippet: OnHandlers


## OmitContentFromException

By default, when a verify mismatch occurs for text, the content of the received and verified files is included in the exception that is thrown. This results in that text being included in test runners and build output. To omit the content use `VerifierSettings.OmitContentFromException`.