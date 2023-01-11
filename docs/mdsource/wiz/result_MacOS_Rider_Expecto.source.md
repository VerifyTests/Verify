# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [MacOS](pickide_MacOS.md) > [JetBrains Rider](picktest_MacOS_Rider.md) > Expecto

## Add NuGet packages

Add the following packages to the test project:

snippet: Expecto-nugets

## Implicit Usings

include: implicit-usings

## Rider Plugin

Install the [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

This is optional, but recommended.## Diff Tool

Verify supports a number of [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
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
