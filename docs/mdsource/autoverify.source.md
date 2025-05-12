# AutoVerify

In some scenarios it makes sense to auto-accept any changes as part of a given test run. For example:

 * Keeping a text representation of a Database schema in a `.verified.sql` file (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).

Note that auto accepted changes in `.verified.` files remain visible in source control tooling.

This can be done using `AutoVerify()`:


### Instance

snippet: AutoVerify


### Fluent

snippet: AutoVerifyFluent


### Globally

snippet: StaticAutoVerify


### With a delegate

AutoVerify supports passing a delegate to control what subset of files to AutoVerify based on file path.


### Instance

snippet: AutoVerifyDelegate


### Fluent

snippet: AutoVerifyFluentDelegate


### Globally

snippet: StaticAutoVerifyDelegate


## Throw when AutoVerify happens

By default, when AutoVerify is being used, the `.verified.` file will be silently replaced with no feedback to the user. 

To get feedback when AutoVerify occurs, use `throwException: true`:

snippet: AutoVerifyThrowException

Using this approach the `.verified.` file will still be replaced, but an exception will be thrown to notify that it has occurred.


## AutoVerify on the build server

By default the same AutoVerify behavior applies both to local test runs and on the build server. To opt out of AutoVerify on the build server use `includeBuildServer: false`:

snippet: AutoVerifyIncludeBuildServer

This can be helpful when the requirement is to minimize the friction of accepting frequent local changes, but once checked in to source control then changes to any verified file should fail a test