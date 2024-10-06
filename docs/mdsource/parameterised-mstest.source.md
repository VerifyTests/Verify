# NUnit Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.MSTest does not detect the parametrised arguments, as such `UseParameters()` is required.

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### DataRow

snippet: MSTestDataRow


## Overriding text used for parameters

include: override-parameters-text


snippet: UseTextForParameters


## Ignore parameters for verified filename

include: ignore-parameters


### MSTest


#### Instance

snippet: IgnoreParametersForVerifiedMsTest


#### Fluent

snippet: IgnoreParametersForVerifiedFluentMsTest


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### MSTest


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsMsTest


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentMsTest


## Hashing parameters

include: hashing-parameters


### MSTest

snippet: UseParametersHashMsTest


### Globally

snippet: StaticHashParameters