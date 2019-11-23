# <img src="/src/icon.png" height="30px"> Verify

[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.svg?cacheSeconds=86400)](https://www.nuget.org/packages/Verify/)

Verification tool to enable simple approval of complex models using [Json.net](https://www.newtonsoft.com/json).

toc

## NuGet package

https://nuget.org/packages/ObjectApproval/


## Usage

Assuming this was verified:

snippet: before

Then attempt to verify this:

snippet: after

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example:

![SampleDiff](/src/SampleDiff.png)

Note that the output is technically not valid json. [Single quotes are used](#single-quotes-used) and [names are not quoted](#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


### Validating multiple instances

When validating multiple instances, an [anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types) can be used for verification

snippet: anon

Results in the following:

snippet: VerifyObjectSamples.Anon.verified.txt


## Serializer settings

Serialization settings can be customized at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.


### Default settings

The default serialization settings are:

snippet: defaultSerialization


### Single quotes used

[JsonTextWriter.QuoteChar](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteChar.htm) is set to single quotes `'`. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences


### QuoteName is false

[JsonTextWriter.QuoteName](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteName.htm) is set to false. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences


### Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior globally use:

```cs
Global.DontIgnoreEmptyCollections();
```


### Guids are scrubbed

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

Results in the following:

snippet: Tests.ShouldReUseGuid.verified.txt

To disable this behavior globally use:

```cs
Global.SontScrubGuids();
```


### Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: Tests.ShouldReUseDatetime.verified.txt

To disable this behavior globally use:

```cs
Global.DontScrubDateTimes();
```


### Default Booleans are ignored

By default values of `bool` and `bool?` are ignored during verification. So properties that equate to 'false' will not be written,

To disable this behavior globally use:

```cs
Global.DontIgnoreFalse();
```


### Change defaults at the verification level

`DateTime`, `DateTimeOffset`, `Guid`, `bool`, and empty collection behavior can also be controlled at the verification level: 

snippet: ChangeDefaultsPerVerification


### Changing settings globally

To change the serialization settings for all verifications use `Global.ApplyExtraSettings()`:

snippet: ExtraSettings


### Scoped settings

snippet: ScopedSerializer

Result:

snippet: VerifyObjectSamples.ScopedSerializer.verified.txt


### Ignoring a type

To ignore all members that match a certain type:

snippet: AddIgnoreType

Result:

snippet: Tests.IgnoreType.verified.txt


### Ignoring a instance

To ignore instances of a type based on delegate:

snippet: AddIgnoreInstance

Result:

snippet: Tests.AddIgnoreInstance.verified.txt


### Ignore member by expressions

To ignore members of a certain type using an expression:

snippet: IgnoreMemberByExpression

Result:

snippet: Tests.IgnoreMemberByExpression.verified.txt


### Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName

Result:

snippet: Tests.IgnoreMemberByName.verified.txt


### Members that throw

Members that throw exceptions can be excluded from serialization based on the exception type or properties.

By default members that throw `NotImplementedException` or `NotSupportedException` are ignored.

Note that this is global for all members on all types.

Ignore by exception type:

snippet: IgnoreMembersThatThrow

Result:

snippet: Tests.CustomExceptionProp.verified.txt

Ignore by exception type and expression:

snippet: IgnoreMembersThatThrowExpression

Result:

snippet: Tests.ExceptionMessageProp.verified.txt



## Named Tuples

Instances of [named tuples](https://docs.microsoft.com/en-us/dotnet/csharp/tuples#named-and-unnamed-tuples) can be verified using `VerifyTuple`.

Due to the use of [ITuple](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.ituple), this approach is only available an net472+ and netcoreapp2.2+.

Given a method that returns a named tuple:

snippet: MethodWithNamedTuple

Can be verified:

snippet: VerifyTuple

Resulting in:

snippet: Tests.NamedTuple.verified.txt


## Scrubbers

Scrubbers run on the final string prior to doing the verification action.

They can be defined at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.

Multiple scrubbers can bee defined at each level.

Scrubber are excited in reveres order. So the most recent added method scrubber through to earlies added global scrubber.

Global scrubbers should be defined only once at appdomain startup.

Usage:

snippet: scrubberssample.cs

Results:

snippet: ScrubbersSample.Simple.verified.txt

snippet: ScrubbersSample.AfterJson.verified.txt


## File extension

The default file extension is `.txt`. So the resulting verified file will be `TestClass.TestMethod.verified.txt`.

It can be overridden at two levels:

 * Method: Change the extension for the current test method.
 * Class: Change the extension all verifications in all test methods for a test class.

Usage:

snippet: ExtensionSample.cs

Result in two files:

snippet: ExtensionSample.InheritedFromClass.verified.json

snippet: ExtensionSample.AtMethod.verified.xml


## Diff Tool

Controlled via environment variables.

 * `VerifyDiffProcess`: The process name. Short name if the tool exists in the current path, otherwise the full path.
 * `VerifyDiffArguments`: The argument syntax to pass to the process. Must contain the strings `{receivedPath}` and `{verifiedPath}`.


### Visual Studio

```
setx VerifyDiffProcess "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe"
setx VerifyDiffArguments "/diff {receivedPath} {verifiedPath}"
```


## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).