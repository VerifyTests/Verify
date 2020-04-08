# Diff Engine

**API SUBJECT TO CHNAGE IN MINOR RELEASES**

DiffEngine contains all functionality used to manage diff tools processes. It is shipped as a stand alone NuGet package: https://www.nuget.org/packages/DiffEngine/. It is designed to be used by [other Snapshot/Approval testing projects](/#alternatives).


## Launching a diff tool

A diff tool can be launched using the following:

snippet: DiffRunnerLaunch

Note that this method will respect the above [difference behavior](#detected-difference-behavior) in terms of Auto refresh and MDI behaviors.


## Closing a diff tool

A diff tool can be closed using the following:

snippet: DiffRunnerKill

Note that this method will respect the above [difference behavior](#detected-difference-behavior) in terms of MDI behavior.


## File type detection

`DiffEngine.Extensions` use data sourced from [sindresorhus/text-extensions](https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json) to determine if a given file or extension is a text file.

Methods:

 * `Extensions.IsTextExtension()` determines if a file extension (without a period `.`) represents a text file.
 * `Extensions.IsTextFile()` determines if a file path represents a text file.