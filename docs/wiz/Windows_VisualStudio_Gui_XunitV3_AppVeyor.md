<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/wiz/Windows_VisualStudio_Gui_XunitV3_AppVeyor.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Windows](Windows.md) > [Visual Studio](Windows_VisualStudio.md) > [Prefer GUI](Windows_VisualStudio_Gui.md) > [XunitV3](Windows_VisualStudio_Gui_XunitV3.md) > AppVeyor

## Add NuGet packages

Add the following packages to the test project:


<!-- snippet: xunitv3-nugets -->
<a id='snippet-xunitv3-nugets'></a>
```csproj
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
<PackageReference Include="Verify.XunitV3" Version="30.4.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.1" PrivateAssets="all" />
<PackageReference Include="xunit.v3" Version="2.0.3" />
```
<sup><a href='/usages/XunitV3NugetUsage/XunitV3NugetUsage.csproj#L8-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-xunitv3-nugets' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Implicit Usings

**All examples use [Implicit Usings](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#implicitusings). Ensure `<ImplicitUsings>` is set to `enable` to ensure examples compile correctly.**<!-- include: implicit-usings. path: /docs/mdsource/implicit-usings.include.md -->

```
<ImplicitUsings>enable</ImplicitUsings>
```

If `ImplicitUsings` are not enabled, substitute usages of `Verify()` with `Verifier.Verify()`.<!-- endInclude -->


## Conventions


### Source Control Includes/Excludes

 * **All `*.received.*` files should be excluded from source control.**<!-- include: include-exclude. path: /docs/mdsource/include-exclude.include.md -->

eg. add the following to `.gitignore`

```
*.received.*
```

If using [UseSplitModeForUniqueDirectory](/docs/naming.md#usesplitmodeforuniquedirectory) also include:

`*.received/`


All `*.verified.*` files should be committed to source control.<!-- endInclude -->


### Text file settings

Text variants of verified and received have the following characteristics:<!-- include: text-file-settings. path: /docs/mdsource/text-file-settings.include.md -->

 * UTF8 with a [Byte order mark (BOM)](https://en.wikipedia.org/wiki/Byte_order_mark)
 * Newlines as line-feed (lf)
 * No trailing newline

This manifests in several ways:


#### Source control settings

All text extensions of `*.verified.*` should have:

 * `eol` set to `lf`
 * `working-tree-encoding` set to `UTF-8`

All Binary files should also be marked to avoid merging and line ending issues with binary files.

eg add the following to `.gitattributes`

```
*.verified.txt text eol=lf working-tree-encoding=UTF-8
*.verified.xml text eol=lf working-tree-encoding=UTF-8
*.verified.json text eol=lf working-tree-encoding=UTF-8
*.verified.bin binary
```


#### EditorConfig settings

If modifying text verified/received files in an editor, it is desirable for the editor to respect the above conventions. For [EditorConfig](https://editorconfig.org/) enabled the following can be used:

```
# Verify settings
[*.{received,verified}.{json,txt,xml}]
charset = utf-8-bom
end_of_line = lf
indent_size = unset
indent_style = unset
insert_final_newline = false
tab_width = unset
trim_trailing_whitespace = false
```

**Note that the above are suggested for subset of text extension. Add others as required based on the text file types being verified.**<!-- endInclude -->


### Conventions check

Conventions can be checked by calling `VerifyChecks.Run()` in a test

<!-- snippet: VerifyChecksXunitV3 -->
<a id='snippet-VerifyChecksXunitV3'></a>
```cs
public class VerifyChecksTests
{
    [Fact]
    public Task Run() =>
        VerifyChecks.Run();
}
```
<sup><a href='/src/Verify.XunitV3.Tests/VerifyChecksTests.cs#L2-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyChecksXunitV3' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


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

<!-- snippet: SampleTestXunitV3 -->
<a id='snippet-SampleTestXunitV3'></a>
```cs
public class Sample
{
    [Fact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
```
<sup><a href='/src/Verify.XunitV3.Tests/Snippets/Sample.cs#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-SampleTestXunitV3' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

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

## Getting .received in output on AppVeyor

Use a [on_failure build step](https://www.appveyor.com/docs/build-configuration/#build-pipeline) to call [Push-AppveyorArtifact](https://www.appveyor.com/docs/build-worker-api/#push-artifact).<!-- include: build-server-appveyor. path: /docs/mdsource/build-server-appveyor.include.md -->

```
on_failure:
  - ps: Get-ChildItem *.received.* -recurse | % { Push-AppveyorArtifact $_.FullName -FileName $_.Name }
```

See also [Pushing artifacts from scripts](https://www.appveyor.com/docs/packaging-artifacts/#pushing-artifacts-from-scripts).<!-- endInclude -->

