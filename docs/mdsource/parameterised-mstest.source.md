# NUnit Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files. The usage depends on the test framework being used:

  * Verify.Expecto: Does not currently support `UseParameters()`.
  * Verify.Fixie: Automatically detects the method parameters via a [custom ITestProject](docs/parameterised.md#fixie).
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


## TUnit

`Verify.TUnit` automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.


### TestCase

snippet: TUnitTestCase


## xUnit V2

`Verify.Xunit` does not detect the parametrized arguments, as such `UseParameters()` is required.


### InlineData

snippet: xunitInlineData


### MemberData

snippet: xunitMemberData


### Complex MemberData

xUnit only exposes parameter information for certain types. For unknown types the information cannot be retrieved from the xUnit context, and instead the text for the parameter value needs to be explicitly specified. This is done by calling `NameForParameter()`.

snippet: xunitComplexMemberData

`VerifierSettings.NameForParameter()` is required since the parameter type has no `ToString()` override that can be used for deriving the name of the `.verified.` file.


## xUnit V3

`Verify.XunitV3` automatically detect the method parameters for built in types (string, int, bool etc), but for complex types `UseParameters()` is required.


### InlineData

snippet: xunitV3InlineData


### MemberData

snippet: xunitV3MemberData


### Complex MemberData

xUnit only exposes parameter information for certain types. For unknown types the information cannot be retrieved from the xUnit context, and instead the text for the parameter value needs to be explicitly specified. This is done by calling `NameForParameter()`.

snippet: xunitV3ComplexMemberData

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

`Verify.MSTest` does not detect the parametrized arguments, as such `UseParameters()` is required.


### DataRow

snippet: MSTestDataRow


include: override-parameters-text


snippet: UseTextForParameters


include: ignore-parameters


### xUnit


#### Instance

snippet: IgnoreParametersForVerifiedXunit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentXunit


### NUnit


#### Instance

snippet: IgnoreParametersForVerifiedNunit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentNunit


### TUnit


#### Instance

snippet: IgnoreParametersForVerifiedTUnit


#### Fluent

snippet: IgnoreParametersForVerifiedFluentTUnit


### MSTest


#### Instance

snippet: IgnoreParametersForVerifiedMsTest


#### Fluent

snippet: IgnoreParametersForVerifiedFluentMsTest


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### xUnit


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsXunit


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentXunit


### NUnit


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsNunit


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentNunit


### TUnit


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsTUnit


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentTUnit


### MSTest


#### Instance

snippet: IgnoreParametersForVerifiedCustomParamsMsTest


#### Fluent

snippet: IgnoreParametersForVerifiedCustomParamsFluentMsTest


include: hashing-parameters


### MSTest

snippet: UseParametersHashMsTest


### NUnit

snippet: UseParametersHashNunit

Note that NUnit can derive the parameters without explicitly passing them.


### TUnit

snippet: UseParametersHashTUnit

Note that TUnit can derive the parameters without explicitly passing them.


### xUnit

snippet: UseParametersHashXunit


### Globally

snippet: StaticHashParameters