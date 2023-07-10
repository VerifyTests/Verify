# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](Windows.md) > [Visual Studio](Windows_VisualStudio.md) > [Prefer CLI](Windows_VisualStudio_Cli.md) > [MSTest](Windows_VisualStudio_Cli_MSTest.md) > GitHub Actions

## Add NuGet packages

Add the following packages to the test project:

```
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package MSTest.TestAdapter
dotnet add package MSTest.TestFramework
dotnet add package Verify.MSTest
```

## Implicit Usings

include: implicit-usings


## Source Control

### Includes/Excludes

include: include-exclude

### Text file settings

include: text-file-settings


## DiffEngineTray

Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

```
dotnet tool install -g DiffEngineTray
```

This is optional, but recommended. Also consider enabling [Run at startup](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md#run-at-startup).

## DiffPlex

The text comparison behavior of Verify is pluggable. The default behaviour, on failure, is to output both the received
and the verified contents as part of the exception. This can be noisy when verifying large strings.

[Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex) changes the text compare result to highlighting text differences inline.

This is optional, but recommended.

### Add the NuGet

```
dotnet add package Verify.DiffPlex
```

### Enable

```cs
[ModuleInitializer]
public static void Initialize() =>
    VerifyDiffPlex.Initialize();
```

## Verify.Terminal

[Verify.Terminal](https://github.com/VerifyTests/Verify.Terminal) is a dotnet tool for managing snapshots from the command line.

This is optional.

### Install the tool

```
dotnet tool install -g verify.tool
```


## Sample Test

snippet: SampleTestMSTest

## Diff Tool

Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

Tools supported by Windows:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [DeltaWalker](https://www.deltawalker.com/)
 * [WinMerge](https://winmerge.org/)
 * [TortoiseGitMerge](https://tortoisegit.org/docs/tortoisegitmerge/)
 * [TortoiseGitIDiff](https://tortoisegit.org/docs/tortoisegitmerge/)
 * [TortoiseMerge](https://tortoisesvn.net/TortoiseMerge.html)
 * [TortoiseIDiff](https://tortoisesvn.net/TortoiseIDiff.html)
 * [KDiff3](https://github.com/KDE/kdiff3)
 * [Guiffy](https://www.guiffy.com/)
 * [ExamDiff](https://www.prestosoft.com/edp_examdiffpro.asp)
 * [Diffinity](https://truehumandesign.se/s_diffinity.php)
 * [Rider](https://www.jetbrains.com/rider/)
 * [Vim](https://www.vim.org/)
 * [Neovim](https://neovim.io/)

## Getting .received in output on GitHub Actions

include: build-server-GitHubActions

