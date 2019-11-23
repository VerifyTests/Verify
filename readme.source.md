# <img src="/src/icon.png" height="30px"> ObjectApproval

[![Build status](https://ci.appveyor.com/api/projects/status/qt5bqw30vp7ywgh3/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/ObjectApproval)
[![NuGet Status](https://img.shields.io/nuget/v/ObjectApproval.svg?cacheSeconds=86400)](https://www.nuget.org/packages/ObjectApproval/)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models using [Json.net](https://www.newtonsoft.com/json).

toc

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