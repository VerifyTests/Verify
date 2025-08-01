<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/wiz/Linux_Rider_Gui_XunitV3_AzureDevOps.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [Linux](Linux.md) > [JetBrains Rider](Linux_Rider.md) > [Prefer GUI](Linux_Rider_Gui.md) > [XunitV3](Linux_Rider_Gui_XunitV3.md) > Azure DevOps

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


## Rider Plugin

Install the [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

This is optional, but recommended.

## Disable orphaned process detection<!-- include: rider-resharper-orphaned-process. path: /docs/mdsource/rider-resharper-orphaned-process.include.md -->

Resharper and Rider have a feature [Check for orphaned processes spawned by test runner](https://www.jetbrains.com/help/resharper/Reference__Options__Tools__Unit_Testing__Test_Runner.html).

> By default, a list of all processes that are launched by the executed tests. If some of theses processes do not exit after the test execution is over, ReSharper will suggest you to terminate the process. If your setup requires some processes started by the tests to continue running, you can clear this checkbox to avoid unnecessary notifications.

Since this project launches diff tools, it will trigger this feature and a dialog will show:

> All unit tests are finished, but child processes spawned by the test runner process are still running. Terminate child process?

<img src="/docs/resharper-spawned.png" alt="R# terminate process dialog" width="400">

As such this feature needs to be disabled:


### Disable for solution

Add the following to `[Solution].sln.DotSettings`.

```
<s:String x:Key="/Default/Housekeeping/UnitTestingMru/UnitTestRunner/SpawnedProcessesResponse/@EntryValue">DoNothing</s:String>
```


### Disable for machine


#### Resharper

ReSharper | Options | Tools | Unit Testing | Test Runner

<img src="/docs/resharper-ignore-spawned.png" alt="Disable R# orphaned processes detection" width="400">


#### Rider

File | Settings | Manage Layers | This computer | Edit Layer | Build, Execution, Deployment | Unit Testing | Test Runner

<img src="/docs/rider-ignore-spawned.png" alt="Disable R# orphaned processes detection" width="500"><!-- endInclude -->


## Treat "return value of pure method is not used" as error

Verify uses the [PureAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.contracts.pureattribute) to mark methods where the result of the method is expected to be used. For example awaiting the call to `Verify()`.<!-- include: pure. path: /docs/mdsource/pure.include.md -->
Rider and ReSharper can be configured to treat an un-used return value as an error.
Add the following to the `.editorconfig` file:

```
[*.cs]
resharper_return_value_of_pure_method_is_not_used_highlighting = error
```
<!-- endInclude -->
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

Tools supported by Linux:

 * [BeyondCompare](https://www.scootersoftware.com)
 * [P4Merge](https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge)
 * [Rider](https://www.jetbrains.com/rider/)
 * [Neovim](https://neovim.io/)

## Getting .received in output on Azure DevOps

Directly after the test runner step add a build step to set a flag if the testrunner failed. This is done by using a [failed condition](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/conditions?view=azure-devops&tabs=yaml). This flag will be evaluated in the CopyFiles and PublishBuildArtifacts steps below.<!-- include: build-server-azuredevops. path: /docs/mdsource/build-server-azuredevops.include.md -->

```yaml
- task: CmdLine@2
  displayName: 'Set flag to publish Verify *.received.* files when test step fails'
  condition: failed()
  inputs:
    script: 'echo "##vso[task.setvariable variable=publishverify]Yes"'
```

Since the PublishBuildArtifacts step in DevOps does not allow a wildcard it is necessary to stage the 'received' files before publishing:

```yaml
- task: CopyFiles@2
  condition: eq(variables['publishverify'], 'Yes')
  displayName: 'Copy Verify *.received.* files to Artifact Staging'
  inputs:
    contents: '**/*.received.*' 
    targetFolder: '$(Build.ArtifactStagingDirectory)/Verify'
    cleanTargetFolder: true
    overWrite: true
```

Publish the staged files as a build artifact:

```yaml
- task: PublishBuildArtifacts@1
  displayName: 'Publish Verify *.received.* files as Artifacts'
  name: 'verifypublish'
  condition: eq(variables['publishverify'], 'Yes')
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)/Verify'
    ArtifactName: 'Verify'
    publishLocation: 'Container'
```
<!-- endInclude -->

