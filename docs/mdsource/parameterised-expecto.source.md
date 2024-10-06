# Expecto Parameterised Tests


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


## Overriding text used for parameters

include: override-parameters-text


snippet: UseTextForParameters


include: ignore-parameters


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


include: hashing-parameters


### Globally

snippet: StaticHashParameters