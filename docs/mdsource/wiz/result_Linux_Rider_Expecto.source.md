# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Linux](pickide_Linux.md) > [JetBrains Rider](picktest_Linux_Rider.md) > Expecto

## Add NuGet packages

Add the following packages to the test project:

snippet: Expecto-nugets

## Implicit Usings

include: implicit-usings

### Includes/Excludes

include: include-exclude


### Line Endings

include: line-endings


## Rider Plugin

Install the [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

This is optional, but recommended.

## Diff Tool

Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

Tool supported by Linux:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [DiffMerge](https://www.sourcegear.com/diffmerge/)
 * [Rider](https://www.jetbrains.com/rider/)
 * [Neovim](https://neovim.io/)
 * [Meld](https://meldmerge.org/)
 * [SublimeMerge](https://www.sublimemerge.com/)
 * [VisualStudioCode](https://code.visualstudio.com)
