# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](pickide_Windows.md) > [Visual Studio with ReSharper](picktest_Windows_VisualStudioWithReSharper.md) > xUnit

## Add NuGet packages

Add the following packages to the test project:

snippet: xUnit-nugets

## Implicit Usings

include: implicit-usings

## Includes/Excludes

include: include-exclude


## Line Endings

include: line-endings


## DiffEngineTray

Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

```
dotnet tool install -g DiffEngineTray
```

This is optional, but recommended. Also consider enabling [Run at startup](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md#run-at-startup).


## ReSharper Plugin

Install the [ReSharper Plugin](https://plugins.jetbrains.com/plugin/17241-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the ReSharper test runner.

This is optional, but recommended.

## Diff Tool

Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

Tool supported by Windows:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [DeltaWalker](https://www.deltawalker.com/)
 * [WinMerge](https://winmerge.org/)
 * [DiffMerge](https://www.sourcegear.com/diffmerge/)
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
 * [AraxisMerge](https://www.araxis.com/merge)
 * [Meld](https://meldmerge.org/)
 * [SublimeMerge](https://www.sublimemerge.com/)
 * [CodeCompare](https://www.devart.com/codecompare/)
 * [VisualStudioCode](https://code.visualstudio.com)
 * [VisualStudio](https://docs.microsoft.com/en-us/visualstudio/ide/reference/diff)
