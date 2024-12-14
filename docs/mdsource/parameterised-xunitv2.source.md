# Xunit V2 Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.Xunit does not detect the parametrised arguments, as such `UseParameters()` is required.


### Usage:

For the above scenarios where parameters are not automatically detected: 

snippet: UseParametersXunit

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSetXunit

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### InlineData


#### Instance

snippet: InlineDataInstanceXunit


#### Fluent

snippet: InlineDataFluentXunit


### MemberData

Given the following MemberData

snippet: MemberDataGetDataXunit


#### Instance

snippet: MemberDataInstanceXunit


#### Fluent

snippet: MemberDataFluentXunit


### Unknown parameter types

include: name-for-parameters

snippet: NameForParametersXunit


## Overriding text used for parameters

include: override-parameters-text


### Instance

snippet: UseTextForParametersInstanceXunit


### Fluent

snippet: UseTextForParametersFluentXunit


## Ignore parameters for verified filename

include: ignore-parameters


### Instance

snippet: IgnoreParametersForVerifiedXunit


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

snippet: UseParametersHashInstanceXunit


### Fluent

snippet: UseParametersHashFluentXunit