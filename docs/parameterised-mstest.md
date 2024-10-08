<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/parameterised-mstest.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# MSTest Parameterised Tests


## UseParameters()

`UseParameters()` controls what parameters are used when naming files.

Verify.MSTest does not detect the parametrised arguments, as such `UseParameters()` is required.

<!-- snippet: UseParameters -->
<a id='snippet-UseParameters'></a>
```cs
[Theory]
[InlineData("Value1")]
[InlineData("Value2")]
public Task UseParametersUsage(string arg)
{
    var somethingToVerify = $"{arg} some text";
    return Verify(somethingToVerify)
        .UseParameters(arg);
}
```
<sup><a href='/src/Verify.XunitV3.Tests/Snippets/ParametersSample.cs#L140-L152' title='Snippet source file'>snippet source</a> | <a href='#snippet-UseParameters' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If not all parameters are required, a subset can be passed in. In this scenario, the parameters passed in will match with the method parameter names from the start. For example the following will result in a file named `ParametersSample.UseParametersSubSet_arg1=Value1_arg2=Value2.verified.txt`

<!-- snippet: UseParametersSubSet -->
<a id='snippet-UseParametersSubSet'></a>
```cs
[Theory]
[InlineData("Value1", "Value2", "Value3")]
public Task UseParametersSubSet(string arg1, string arg2, string arg3)
{
    var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
    return Verify(somethingToVerify)
        .UseParameters(arg1, arg2);
}
```
<sup><a href='/src/Verify.XunitV3.Tests/Snippets/ParametersSample.cs#L154-L165' title='Snippet source file'>snippet source</a> | <a href='#snippet-UseParametersSubSet' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If the number of parameters passed to `UseParameters()` is greater than the number of parameters in the test method, an exception will be thrown.


### DataRow

<!-- snippet: MSTestDataRow -->
<a id='snippet-MSTestDataRow'></a>
```cs
[DataTestMethod]
[DataRow("Value1")]
[DataRow("Value2")]
public Task DataRowUsage(string arg)
{
    var settings = new VerifySettings();
    settings.UseParameters(arg);
    return Verify(arg, settings);
}

[DataTestMethod]
[DataRow("Value1")]
[DataRow("Value2")]
public Task DataRowUsageFluent(string arg) =>
    Verify(arg)
        .UseParameters(arg);
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/ParametersSample.cs#L4-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-MSTestDataRow' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Overriding text used for parameters

`UseTextForParameters()` can be used to override the substitution text used for the `{Parameters}` part of the file convention.<!-- include: override-parameters-text. path: /docs/mdsource/override-parameters-text.include.md -->

```
{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}
```

The below samples produce:

For the instance case:

 * TheTest.UseTextForParameters_Value1.verified.txt
 * TheTest.UseTextForParameters_Value2.verified.txt

For the fluent case:

 * TheTest.UseTextForParametersFluent_Value1.verified.txt
 * TheTest.UseTextForParametersFluent_Value2.verified.txt<!-- endInclude -->


<!-- snippet: UseTextForParameters -->
<a id='snippet-UseTextForParameters'></a>
```cs
[Theory]
[InlineData("Value1")]
[InlineData("Value2")]
public Task UseTextForParameters(string arg)
{
    var settings = new VerifySettings();
    settings.UseTextForParameters(arg);
    return Verify(arg + "UseTextForParameters", settings);
}

[Theory]
[InlineData("Value1")]
[InlineData("Value2")]
public Task UseTextForParametersFluent(string arg) =>
    Verify(arg + "UseTextForParametersFluent")
        .UseTextForParameters(arg);
```
<sup><a href='/src/Verify.Tests/Naming/NamerTests.cs#L381-L400' title='Snippet source file'>snippet source</a> | <a href='#snippet-UseTextForParameters' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Ignore parameters for verified filename

By default, Verify expects every parameterized case to have a unique [file name](/docs/naming.md) with the parameters appended to the file name. This behavior can be overridden by using `IgnoreParametersForVerified()`. In this case, the verified file name does not contain the parameter values, meaning it is the same for each testcase.<!-- include: ignore-parameters. path: /docs/mdsource/ignore-parameters.include.md -->

`IgnoreParametersForVerified` accepts an array for passing through the parameters. These values are passed to [UseParameters](#UseParameters). This is required for MSTest, and xUnit. Parameters should not be passed for NUnit, TUnit and Fixie since they are automatically detected.

The below samples produce:

For the instance case:

 * NamerTests.IgnoreParametersForVerified_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerified_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerified.verified.txt

For the fluent case:

 * NamerTests.IgnoreParametersForVerifiedFluent_arg=One.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent_arg=Two.received.txt
 * NamerTests.IgnoreParametersForVerifiedFluent.verified.txt<!-- endInclude -->


### Instance

<!-- snippet: IgnoreParametersForVerifiedMsTest -->
<a id='snippet-IgnoreParametersForVerifiedMsTest'></a>
```cs
[DataTestMethod]
[DataRow("One")]
[DataRow("Two")]
public Task IgnoreParametersForVerified(string arg)
{
    var settings = new VerifySettings();
    settings.IgnoreParametersForVerified(arg);
    return Verify("value", settings);
}
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/ParametersSample.cs#L25-L37' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreParametersForVerifiedMsTest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Fluent

<!-- snippet: IgnoreParametersForVerifiedFluentMsTest -->
<a id='snippet-IgnoreParametersForVerifiedFluentMsTest'></a>
```cs
[DataTestMethod]
[DataRow("One")]
[DataRow("Two")]
public Task IgnoreParametersForVerifiedFluent(string arg) =>
    Verify("value")
        .IgnoreParametersForVerified(arg);
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/ParametersSample.cs#L39-L48' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreParametersForVerifiedFluentMsTest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## IgnoreParametersForVerified with override parameters

The parameters passed to IgnoreParametersForVerified can be used pass custom parameters to [UseParameters](#UseParameters).


### Instance

<!-- snippet: IgnoreParametersForVerifiedCustomParamsMsTest -->
<a id='snippet-IgnoreParametersForVerifiedCustomParamsMsTest'></a>
```cs
[DataTestMethod]
[DataRow("One")]
[DataRow("Two")]
public Task IgnoreParametersForVerifiedCustomParams(string arg)
{
    var settings = new VerifySettings();
    settings.IgnoreParametersForVerified($"Number{arg}");
    return Verify("value", settings);
}
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/ParametersSample.cs#L50-L62' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreParametersForVerifiedCustomParamsMsTest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Fluent

<!-- snippet: IgnoreParametersForVerifiedCustomParamsFluentMsTest -->
<a id='snippet-IgnoreParametersForVerifiedCustomParamsFluentMsTest'></a>
```cs
[DataTestMethod]
[DataRow("One")]
[DataRow("Two")]
public Task IgnoreParametersForVerifiedFluentCustomParams(string arg) =>
    Verify("value")
        .IgnoreParametersForVerified($"Number{arg}");
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/ParametersSample.cs#L64-L73' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreParametersForVerifiedCustomParamsFluentMsTest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Hashing parameters

Parameters can be hashed as an alternative to being stringified. This is useful when the parameters are large and could potentially generate file names that exceed allowances of the OS.<!-- include: hashing-parameters. path: /docs/mdsource/hashing-parameters.include.md -->

Hashing parameter is achieved by using `UseParameters` in combination with `HashParameters`. Alternatively `UseHashedParameters` can be used as a wrapper for those two method calls.

[Overriding text used for parameters](#overriding-text-used-for-parameters) is respected when generating the hash.

[XxHash64](https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.xxhash64) is used to perform the hash.<!-- endInclude -->
