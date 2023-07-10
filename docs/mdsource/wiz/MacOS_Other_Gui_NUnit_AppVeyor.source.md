# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [MacOS](MacOS.md) > [Other](MacOS_Other.md) > [Prefer GUI](MacOS_Other_Gui.md) > [NUnit](MacOS_Other_Gui_NUnit.md) > AppVeyor

## Add NuGet packages

Add the following packages to the test project:


snippet: NUnit-nugets


## Implicit Usings

include: implicit-usings


## Source Control

### Includes/Excludes

include: include-exclude

### Text file settings

include: text-file-settings

## DiffPlex

The text comparison behavior of Verify is pluggable. The default behaviour, on failure, is to output both the received
and the verified contents as part of the exception. This can be noisy when verifying large strings.

[Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex) changes the text compare result to highlighting text differences inline.

This is optional, but recommended.

### Add the NuGet

```xml
<PackageReference Include="Verify.DiffPlex" Version="*" />
```

### Enable

```cs
[ModuleInitializer]
public static void Initialize() =>
    VerifyDiffPlex.Initialize();
```


## Sample Test

snippet: SampleTestNUnit

## Diff Tool

Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

Tools supported by MacOS:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [Kaleidoscope](https://kaleidoscope.app)
 * [DeltaWalker](https://www.deltawalker.com/)
 * [KDiff3](https://github.com/KDE/kdiff3)
 * [TkDiff](https://sourceforge.net/projects/tkdiff/)
 * [Guiffy](https://www.guiffy.com/)
 * [Rider](https://www.jetbrains.com/rider/)
 * [Vim](https://www.vim.org/)
 * [Neovim](https://neovim.io/)

## Getting .received in output on AppVeyor

include: build-server-AppVeyor

