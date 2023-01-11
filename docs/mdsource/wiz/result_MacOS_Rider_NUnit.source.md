# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [MacOS](pickide_MacOS.md) > [JetBrains Rider](picktest_MacOS_Rider.md) > NUnit

## Add NuGet packages

Add the following packages to the test project:

snippet: NUnit-nugets

## Implicit Usings

include: implicit-usings

## Includes/Excludes

include: include-exclude


## Line Endings

include: line-endings


## Rider Plugin

Install the [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

This is optional, but recommended.

## DiffPlex

The text comparison behavior of Verify is pluggable. The default behaviour, on failure, is to output both the received
and the verified contents as part of the exception. This can be noisy when verifying large strings.

[Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex) changes the text compare result to be more highlighting text changes inline.

This is optional, but recommended.


### Add the NuGet

```
<PackageReference Include="Verify.DiffPlex" Version="*" />
```


### Enable

```
[ModuleInitializer]
public static void Initialize() =>
    VerifyDiffPlex.Initialize();
```

## Diff Tool

Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

Tool supported by MacOS:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [Kaleidoscope](https://www.kaleidoscopeapp.com/)
 * [DeltaWalker](https://www.deltawalker.com/)
 * [DiffMerge](https://www.sourcegear.com/diffmerge/)
 * [KDiff3](https://github.com/KDE/kdiff3)
 * [TkDiff](https://sourceforge.net/projects/tkdiff/)
 * [Guiffy](https://www.guiffy.com/)
 * [Rider](https://www.jetbrains.com/rider/)
 * [Vim](https://www.vim.org/)
 * [Neovim](https://neovim.io/)
 * [AraxisMerge](https://www.araxis.com/merge)
 * [Meld](https://meldmerge.org/)
 * [SublimeMerge](https://www.sublimemerge.com/)
 * [VisualStudioCode](https://code.visualstudio.com)
