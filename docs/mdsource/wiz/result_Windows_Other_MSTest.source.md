# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](pickide_Windows.md) > [Other](picktest_Windows_Other.md) > MSTest

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