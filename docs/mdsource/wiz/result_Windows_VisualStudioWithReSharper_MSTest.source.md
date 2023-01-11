# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](pickide_Windows.md) > [Visual Studio with ReSharper](picktest_Windows_VisualStudioWithReSharper.md) > MSTest

## Add NuGet packages

Add the following packages to the test project:

snippet: MSTest-nugets

## Implicit Usings

include: implicit-usings

## DiffEngineTray

Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

```
dotnet tool install -g DiffEngineTray
```

This is optional, but recommended.
## ReSharper Plugin

Install the [ReSharper Plugin](https://plugins.jetbrains.com/plugin/17241-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the ReSharper test runner.

This is optional, but recommended.