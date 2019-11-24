# <img src="/src/icon.png" height="30px"> Verify

[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Xunit.svg?cacheSeconds=86400)](https://www.nuget.org/packages/Verify.Xunit/)

Verification tool to enable simple approval of complex models using [Json.net](https://www.newtonsoft.com/json).

toc

## NuGet package

https://nuget.org/packages/Verify.Xunit/


## Usage

Given a class to be tested:

snippet: ClassBeingTested

It can be tested as follows:

snippet: SampleTest

When run the test will fail with:

> First verification. SampleTest.Simple.verified.txt not found. Verification command has been copied to the clipboard.

The clipboard will contain the following:

> cmd /c move /Y "C:\Code\Sample\SampleTest.Simple.received.txt" "C:\Code\Sample\SampleTest.Simple.verified.txt"

If a [Diff Tool](docs/diff-tool.md) is enable it will display the diff:

![SampleDiff](/src/InitialDiff.png)

To verify the result:

 * Execute the command from the clipboard, or
 * Use the diff tool to accept the changes , or
 * Manually copy the text to the new file

This will result in the following file being created:

snippet: SampleTest.Simple.verified.txt




The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example:



## Not valid json

Note that the output is technically not valid json. [Single quotes are used](docs/serializer-settings.md#single-quotes-used) and [names are not quoted](docs/serializer-settings.md#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


## Validating multiple instances

When validating multiple instances, an [anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types) can be used for verification

snippet: anon

Results in the following:

snippet: VerifyObjectSamples.Anon.verified.txt


## Documentation

 * [Serializer Settings](docs/serializer-settings.md)
 * [File Extension](docs/file-extension.md)
 * [Named Tuples](docs/named-tuples.md)
 * [Scrubbers](docs/scrubbers.md)
 * [Diff Tool](docs/diff-tool.md)


## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).