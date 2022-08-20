# Parameterised Tests


## Additions to file name

Every parameterised case has a unique [file name](/docs/naming.md) with the parameters appended to the file name.

The appending format is `_ParamName=ParamValue` repeated for each parameter. 

A test with two parameters `param1` + `param2`, and called twice with the values `value1a`+ `value2a` and `value1b`+ `value2b` would have the following file names:

  * `MyTest.MyMethod_param1=value1a_param2=value2a.verified.txt`
  * `MyTest.MyMethod_param1=value1b_param2=value2b.verified.txt`


### Invalid characters

Characters that cannot be used for a file name will be replaced with a dash (`-`).


## UseParameters

`UseParameters` is used to control what parameters are used when naming files. The usual usage is to pass though all parameters (in the same order) that the test method accepts:

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters pass to `UseParameters` is greater than the number of parameters in the test method, an exception will be thrown.


## xUnit


### InlineData

snippet: xunitInlineData


### MemberData

snippet: xunitMemberData


### Complex MemberData

xUnit only exposes parameter information when the types certain types. For unknown types the parameter information cannot be retrieved from the xUnit context, and instead the parameters need to be explicitly passed in. This is done by calling `UseParameters()`.

snippet: xunitComplexMemberData

`VerifierSettings.NameForParameter` is required since the parameter type has no `ToString()` override that can be used for deriving the name of the `.verified.` file.


## NUnit


### TestCase

snippet: NUnitTestCase


### TestFixtureSourceUsage

When using a [TestFixtureSource](https://docs.nunit.org/articles/nunit/writing-tests/attributes/testfixturesource.html) the the name provided by NUnit will be as the `TestMethodName`.

snippet: TestFixtureSourceUsage.cs

Produces `TestFixtureSourceUsage(Value1,1).Test.verified.txt` and `TestFixtureSourceUsage(Value2,2).Test.verified.txt`.


## MSTest

MSTest does not expose the parameter values via its extensibility context. So parameter values must passed in via settings.


### DataRow

snippet: MSTestDataRow


## Overriding text used for parameters

`UseTextForParameters()` can be used to override the text used for `{Parameters}`.

```
{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}
```

snippet: UseTextForParameters

Results in:

 * TheTest.UseTextForParameters_Value1.verified.txt
 * TheTest.UseTextForParameters_Value2.verified.txt
 * TheTest.UseTextForParametersFluent_Value1.verified.txt
 * TheTest.UseTextForParametersFluent_Value2.verified.txt


## Ignore parameters for verified filename

By default, every parameterised case has a unique [file name](/docs/naming.md) with the parameters appended to the file name. This behavior can be overridden by using `IgnoreParametersForVerified()`. In this case, the verified file name does not contain the parameter values, meaning it is the same for each testcase.

snippet: IgnoreParametersForVerified

Results in:

* NamerTests.IgnoreParametersForVerified_arg=One.received.txt
* NamerTests.IgnoreParametersForVerified_arg=Two.received.txt
* NamerTests.IgnoreParametersForVerified.verified.txt

And for the second test:

* NamerTests.IgnoreParametersForVerifiedFluent_arg=One.received.txt
* NamerTests.IgnoreParametersForVerifiedFluent_arg=Two.received.txt
* NamerTests.IgnoreParametersForVerifiedFluent.verified.txt
