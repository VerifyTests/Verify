# Xunit V2 Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.Xunit does not detect the parametrised arguments, as such `UseParameters()` is required.


### Usage:

For the above scenarios where parameters are not automatically detected: 

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### InlineData

snippet: xunitInlineData


### MemberData

snippet: xunitMemberData


### Complex MemberData

xUnit only exposes parameter information for certain types. For unknown types the information cannot be retrieved from the xUnit context, and instead the text for the parameter value needs to be explicitly specified. This is done by calling `NameForParameter()`.

snippet: xunitComplexMemberData

`VerifierSettings.NameForParameter()` is required since the parameter type has no `ToString()` override that can be used for deriving the name of the `.verified.` file.


## Overriding text used for parameters

include: override-parameters-text


snippet: UseTextForParameters


## Ignore parameters for verified filename

include: ignore-parameters


### xUnit


#### Instance

snippet: IgnoreParametersForVerifiedXunit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentXunit



## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### Instance

snippet: IgnoreParametersForVerifiedCustomParamsXunit


### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentXunit


## Hashing parameters

include: hashing-parameters

snippet: UseParametersHashXunit