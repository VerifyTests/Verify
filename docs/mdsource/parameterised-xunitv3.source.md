# Xunit V3 Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.XunitV3 Automatically detect the method parameters for built in types (string, int, bool etc), but for complex parameters `UseParameters()` is required.


### Usage:

For the above scenarios where parameters are not automatically detected: 

snippet: UseParametersXunitV3

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSetXunitV3

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### InlineData

snippet: InlineDataXunitV3


### MemberData

snippet: MemberDataXunitV3


### Unknown parameter types

include: name-for-parameters

snippet: NameForParametersXunitV3


## Overriding text used for parameters

include: override-parameters-text


### Instance

snippet: UseTextForParametersInstanceXunitV3


### Fluent

snippet: UseTextForParametersFluentXunitV3


## Ignore parameters for verified filename

include: ignore-parameters


### Instance

snippet: IgnoreParametersForVerifiedXunitV3


### Fluent

snippet: IgnoreParametersForVerifiedFluentXunit


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### Instance

snippet: IgnoreParametersForVerifiedCustomParamsXunit


### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentXunit


## Hashing parameters

include: hashing-parameters


### Instance

snippet: UseParametersHashInstanceXunitV3


### Fluent

snippet: UseParametersHashFluentXunitV3