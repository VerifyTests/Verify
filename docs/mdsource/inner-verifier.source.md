# InnerVerifier

InnerVerifier provides low level access to the verification engine. It is exposed publicly for allowing test frameworks to consume Verify, in a similar way to the bundled extensions for MsTest, nUnit and xUnit. It is not intended for top level consumption of people writing tests.


## Required information

InnerVerifier requires the following information to run a snapshot test.


### Method and type

The `Type` and `MethodInfo` contribute the [File Name](/docs/naming.md).

Depending on test framework this is derived from static or injected state.


### The test file path.

The file path is contributes to the [File Name](/docs/naming.md) and is usually done with a `CallerFilePath`.

snippet: GetFilePath

This path would normally be passed in on each of the Verify method overloads. For example [the Verify file overload in Xunit](https://github.com/VerifyTests/Verify/blob/main/src/Verify.Xunit/Verifier_File.cs).


### VerifySettings

An instance of `VerifySettings` for configuring the verification behavior. It is also used to pass through any parameters used by the test.


## Examples

snippet: RawUsage

snippet: RawUsageWithParams


## Existing extensions


### Xunit


#### Static approach

 * [InnerVerifier construction](https://github.com/VerifyTests/Verify/blob/main/src/Verify.Xunit/Verifier.cs)
 * [Example verify overload: File](https://github.com/VerifyTests/Verify/blob/main/src/Verify.Xunit/Verifier_File.cs)


#### Base class approach

 * [InnerVerifier construction](https://github.com/VerifyTests/Verify/blob/main/src/Verify.Xunit/VerifyBase.cs)
 * [Example verify overload: File](https://github.com/VerifyTests/Verify/blob/main/src/Verify.Xunit/VerifyBase_File.cs)


### NUnit


#### Static approach

 * [InnerVerifier construction](https://github.com/VerifyTests/Verify/blob/main/src/Verify.NUnit/Verifier.cs)
 * [Example verify overload: File](https://github.com/VerifyTests/Verify/blob/main/src/Verify.NUnit/Verifier_File.cs)


#### Base class approach

 * [InnerVerifier construction](https://github.com/VerifyTests/Verify/blob/main/src/Verify.NUnit/VerifyBase.cs)
 * [Example verify overload: File](https://github.com/VerifyTests/Verify/blob/main/src/Verify.NUnit/VerifyBase_File.cs)


### MSTest

MSTest has no static approach since it does not expose a static test context.

#### Base class approach

 * [InnerVerifier construction](https://github.com/VerifyTests/Verify/blob/main/src/Verify.MSTest/VerifyBase.cs)
 * [Example verify overload: File](https://github.com/VerifyTests/Verify/blob/main/src/Verify.MSTest/VerifyBase_File.cs)