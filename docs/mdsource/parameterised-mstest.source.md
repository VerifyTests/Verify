# MSTest Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.MSTest automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSetMSTest

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### DataRow

snippet: DataRowInstanceMSTest


## Overriding text used for parameters

include: override-parameters-text

snippet: UseTextForParametersMSTest


## Ignore parameters for verified filename

include: ignore-parameters


### Instance

snippet: IgnoreParametersForVerifiedMSTest


### Fluent

snippet: IgnoreParametersForVerifiedFluentMSTest


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### Instance

snippet: IgnoreParametersForVerifiedCustomParamsMSTest


### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentMSTest