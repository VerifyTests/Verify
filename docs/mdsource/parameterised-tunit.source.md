# TUnit Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files. 

Verify.TUnit automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.


### Usage:

For the above scenarios where parameters are not automatically detected: 

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### TestCase

snippet: TUnitTestCase


## Overriding text used for parameters

include: override-parameters-text


snippet: UseTextForParameters


## Ignore parameters for verified filename

include: ignore-parameters


### TUnit


#### Instance

snippet: IgnoreParametersForVerifiedTUnit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentTUnit


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### Instance

snippet: IgnoreParametersForVerifiedCustomParamsTUnit


### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentTUnit


## Hashing parameters

include: hashing-parameters


snippet: UseParametersHashTUnit

Note that TUnit can derive the parameters without explicitly passing them.


### Globally

snippet: StaticHashParameters