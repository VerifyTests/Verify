By default, Verify expects every parameterized case to have a unique [file name](/docs/naming.md) with the parameters appended to the file name. This behavior can be overridden by using `IgnoreParametersForVerified()`. In this case, the verified file name does not contain the parameter values, meaning it is the same for each testcase.

`IgnoreParametersForVerified` accepts an array for passing through the parameters. These values are passed to [UseParameters](#UseParameters). This is required for MSTest, and xUnit. Parameters should not be passed for NUnit, TUnit and Fixie since they are automatically detected.

The below samples produce:

For the instance case:

 * NamerTests.IgnoreParametersForVerified_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerified_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerified.verified.txt

For the fluent case:

 * NamerTests.IgnoreParametersForVerifiedFluent_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent.verified.txt


### Static IgnoreParameters

`VerifierSettings.IgnoreParameters()` can be used to globally ignore specific parameters (by name) for all tests. This is useful when a parameter is non-deterministic or irrelevant to the verified output across the entire test suite. It must be called before any test runs, typically in a `[ModuleInitializer]`.

When passing an empty list, all parameters will be ignored. When passing specific parameter names, only those parameters will be excluded from the verified filename. Parameters that do not exist on a given test method are silently skipped.

The received files still contain all parameter values.

```cs
[ModuleInitializer]
public static void Init() =>
    VerifierSettings.IgnoreParameters("arg");
```
