# NUnit Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files. The usage depends on the test framework being used:

  * Verify.Expecto: Does not currently support `UseParameters()`.
  * Verify.Fixie: Automatically detects the method parameters via a [custom ITestProject]( docs/parameterised.md#fixie).
  * Verify.MSTest: Does not detect the parametrised arguments, as such `UseParameters()` is required.
  * Verify.NUnit: Automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.
  * Verify.TUnit: Automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.
  * Verify.Xunit: Does not detect the parametrised arguments, as such `UseParameters()` is required.
  * Verify.XunitV3: Automatically detect the method parameters for built in types (string, int, bool etc), but for complex parameters `UseParameters()` is required.


### Usage:

For the above scenarios where parameters are not automatically detected: 

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


## NUnit

`Verify.NUnit` automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.


### TestCase

snippet: NUnitTestCase


### TestFixtureSourceUsage

When using a [TestFixtureSource](https://docs.nunit.org/articles/nunit/writing-tests/attributes/testfixturesource.html) the name provided by NUnit will be as the `TestMethodName`.

snippet: TestFixtureSourceUsage.cs

Produces:

 * `TestFixtureSourceUsage.Test_arg1=Value1_arg2=1.verified.txt`
 * `TestFixtureSourceUsage.Test_arg1=Value2_arg2=2.verified.txt`


## Overriding text used for parameters

include: override-parameters-text


snippet: UseTextForParameters


include: ignore-parameters


### NUnit


#### Instance

snippet: IgnoreParametersForVerifiedNunit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentNunit


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### NUnit


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsNunit


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentNunit


include: hashing-parameters


### MSTest

snippet: UseParametersHashMsTest


### NUnit

snippet: UseParametersHashNunit

Note that NUnit can derive the parameters without explicitly passing them.



### Globally

snippet: StaticHashParameters