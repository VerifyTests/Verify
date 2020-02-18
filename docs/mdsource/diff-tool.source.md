# Diff Tool

When a test fails verification the difference between the received and verified files is displayed in a diff tool.


## Initial difference behavior

Behavior when an input is verified for the first time.

Behavior depends on if an [EmptyFiles](https://github.com/SimonCropp/EmptyFiles) can be found matching the current extension.

 * If an EmptyFiles can be found matching the current extension, then the diff tool will be launch to compare the input to that empty file.
 * If no EmptyFiles can be found no diff tool will be launched.


## Detected difference behavior

Behavior when a difference is detected between the input an existing current verified file.


### Not Running

If no diff tool is running for the comparison of the current verification (per test), a new diff tool instance will be launched.


### Is Running

If a diff tool is running for the comparison of the current verification (per test), and a new verification fails, the following logic will be applied:

| Auto Refresh | Mdi   | Behavior |
|--------------|-------|----------|
| true         | true  | No action. Current instance will refresh |
| true         | false | No action. Current instance will refresh |
| false        | true  | Open new instance. Previous instance must be manually closed |
| false        | false | Kill current and open new instance |

include: diffToolCleanup


## Successful verification behavior

If a diff tool is running for the comparison of the current verification (per test), and a new verification passes, the following logic will be applied:

| Mdi   | Behavior |
|-------|----------|
| true  | No action taken. Previous instance must be manually closed |
| false | Kill current instance |

include: diffToolCleanup


## Supported Diff tools:

include: diffTools


## Disable Diff

snippet: DisableDiff


## DiffEngine

**API SUBJECT TO CHNAGE IN MINOR RELEASES**

DiffEngine contains all functionality used to manage diff tools processes. It is shipped as a stand alone NuGet package: https://www.nuget.org/packages/DiffEngine/. It is designed to be used by [other Snapshot/Approval testing projects](/#alternatives).


### Launching a diff tool

A diff tool can be launched using the following:

snippet: DiffRunnerLaunch

Note that this method will respect the above [difference behavior](#detected-difference-behavior) in terms of Auto refresh and MDI behaviors.


### Closing a diff tool

A diff tool can be closed using the following:

snippet: DiffRunnerKill

Note that this method will respect the above [difference behavior](#detected-difference-behavior) in terms of MDI behavior.


### File type detection

`DiffEngine.Extensions` use data sourced from [sindresorhus/text-extensions](https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json) to determine if a given file or extension is a text file.

Methods:

 * `Extensions.IsTextExtension()` determines if a file extension (without a period `.`) represents a text file.
 * `Extensions.IsTextFile()` determines if a file path represents a text file.