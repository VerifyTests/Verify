# <img src="/src/icon.png" height="30px"> Verify Options

## AutoVerify

In some scenarios it makes sense to auto-accept any changes as part of a given test run. For example:

 * Keeping a text representation of a Database schema in a `.verified.sql` file (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).

This can be done using `AutoVerify()`:

snippet: AutoVerify

Note that auto accepted changes in `.verified.` files remain visible in source control tooling.


### OnHandlers

`OnFirstVerify` is called when there is no verified file.

`OnVerifyMismatch` is called when a received file does not match the existing verified file.

snippet: OnHandlers