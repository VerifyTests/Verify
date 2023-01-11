# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](pickide_Windows.md) > [JetBrains Rider](picktest_Windows_Rider.md) > xUnit

### Add NuGet packages

Add the following packages to the test project:

snippet: xUnit-nugets

### Implicit Usings

include: implicit-usings

## DiffEngineTray

Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

```
dotnet tool install -g DiffEngineTray
```

This is optional, but recommended.
## Rider Plugin

Install [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

This is optional, but recommended.