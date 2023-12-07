# Parameterised Tests


## Additions to file name

Every parameterised case should have a unique [file name](/docs/naming.md) with the parameters appended to the file name. This happens automatically for NUnit; xUnit and MSTest require the use of `UseParameters()` (see below).

The appending format is `_ParamName=ParamValue` repeated for each parameter. 

A test with two parameters `param1` + `param2`, and called twice with the values `value1a`+ `value2a` and `value1b`+ `value2b` would have the following file names:

  * `MyTest.MyMethod_param1=value1a_param2=value2a.verified.txt`
  * `MyTest.MyMethod_param1=value1b_param2=value2b.verified.txt`


### Invalid characters

Characters that cannot be used for a file name are replaced with a dash (`-`).


## UseParameters()

<b>`Verify.NUnit` and `Verify.Fixie` automatically detect the method parameters. So `UseParameters()` is not required unless using custom parameters.</b>

`UseParameters`() is used to control what parameters are used when naming files. The usual usage is to pass though all parameters (in the same order) that the test method accepts:

snippet: UseParameters

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

snippet: UseParametersSubSet

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


## NUnit


### TestCase

snippet: NUnitTestCase


### TestFixtureSourceUsage

When using a [TestFixtureSource](https://docs.nunit.org/articles/nunit/writing-tests/attributes/testfixturesource.html) the the name provided by NUnit will be as the `TestMethodName`.

snippet: TestFixtureSourceUsage.cs

Produces `TestFixtureSourceUsage(Value1,1).Test.verified.txt` and `TestFixtureSourceUsage(Value2,2).Test.verified.txt`.


## xUnit


### InlineData

snippet: xunitInlineData


### MemberData

snippet: xunitMemberData


### Complex MemberData

xUnit only exposes parameter information for certain types. For unknown types the information cannot be retrieved from the xUnit context, and instead the text for the parameter value needs to be explicitly specified. This is done by calling `NameForParameter()`.

snippet: xunitComplexMemberData

`VerifierSettings.NameForParameter()` is required since the parameter type has no `ToString()` override that can be used for deriving the name of the `.verified.` file.


## Fixie

Fixie has no build in test parameterisation. Test parameterisation need to be implemented by the consuming library. See [Attribute-Based Parameterization](https://github.com/fixie/fixie/wiki/Customizing-the-Test-Project-Lifecycle#recipe-attribute-based-parameterization) for an example.

Verify.Fixie requires some customisation of the above example.

 * Inside `ITestProject.Configure` call `VerifierSettings.AssignTargetAssembly(environment.Assembly);`
 * Inside `IExecution.Run` wrap `test.Run` in `using (ExecutionState.Set(testClass, test, parameters))`

Example implementation:

snippet: TestProject.cs

Resulting usage:

snippet: FixieTestCase


## MSTest


### DataRow

snippet: MSTestDataRow


## Overriding text used for parameters

`UseTextForParameters()` can be used to override the substitution text used for `{Parameters}`.

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

By default, Verify expects every parameterised case to have a unique [file name](/docs/naming.md) with the parameters appended to the file name. This behavior can be overridden by using `IgnoreParametersForVerified()`. In this case, the verified file name does not contain the parameter values, meaning it is the same for each testcase.

snippet: IgnoreParametersForVerified

Results in:

 * NamerTests.IgnoreParametersForVerified_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerified_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerified.verified.txt

And for the second test:

 * NamerTests.IgnoreParametersForVerifiedFluent_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent.verified.txt


## Hashing parameters

Parameters can be hashed as an alternative to being stringified. This is useful when the parameters are large and could potentially generate file names that exceed allowances of the OS.

Hashing parameter is achieved by using `UseParameters` in combination with `HashParameters`. Alternatively `UseHashedParameters` can be used as a wrapper for those two method calls.

[Overriding text used for parameters](#overriding-text-used-for-parameters) is respected when generating the hash.

[XxHash64](https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.xxhash64) is used to perform the hash.


### MSTest

snippet: UseParametersHashMsTest


### NUnit

snippet: UseParametersHashNunit

Note that NUnit can derive the parameters without explicitly passing them.


### xUnit

snippet: UseParametersHashxUnit