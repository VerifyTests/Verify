<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/wiz/result_MacOS_Other_MSTest.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# Getting Started Wizard

[Home](/docs/wiz/readme.md) > [MacOS](pickide_MacOS.md) > [Other](picktest_MacOS_Other.md) > MSTest

### Add NuGet packages

Add the following packages to the test project:

<!-- snippet: MSTest-nugets -->
<a id='snippet-mstest-nugets'></a>
```csproj
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.4.1" />
<PackageReference Include="MSTest.TestAdapter" Version="3.0.2" />
<PackageReference Include="MSTest.TestFramework" Version="3.0.2" />
<PackageReference Include="Verify.MSTest" Version="19.6.0" />
```
<sup><a href='/src/NugetUsage/MSTestNugetUsage/MSTestNugetUsage.csproj#L7-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-mstest-nugets' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Implicit Usings

**All examples use [ImplicitUsings](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#implicitusings). Ensure the following is set to have examples compile correctly `<ImplicitUsings>enable</ImplicitUsings>`** <!-- include: implicit-usings. path: /docs/implicit-usings.include.md -->

If `ImplicitUsings` are not enabled, substitute usages of `Verify()` with `Verifier.Verify()`. <!-- endInclude -->