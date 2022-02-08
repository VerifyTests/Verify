# <img src='/src/icon.png' height='30px'> Verify

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/VerifyTests/Discussions/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Xunit.svg?label=Verify.Xunit)](https://www.nuget.org/packages/Verify.Xunit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.NUnit.svg?label=Verify.NUnit)](https://www.nuget.org/packages/Verify.NUnit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Expecto.svg?label=Verify.Expecto)](https://www.nuget.org/packages/Verify.Expecto/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.MSTest.svg?label=Verify.MSTest)](https://www.nuget.org/packages/Verify.MSTest/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.MSTest.svg?label=Verify.ClipboardAccept)](https://www.nuget.org/packages/Verify.ClipboardAccept/)

Verify is a snapshot tool that simplifies the assertion of complex data models and documents.

Verify is called on the test result during the assertion phase. It serializes that result and stores it in a file that matches the test name. On the next test execution, the result is again serialized and compared to the existing file. The test will fail if the two snapshots do not match: either the change is unexpected, or the reference snapshot needs to be updated to the new result.


## NuGet packages

 * https://nuget.org/packages/Verify.Xunit/
 * https://nuget.org/packages/Verify.NUnit/
 * https://nuget.org/packages/Verify.Expecto/
 * https://nuget.org/packages/Verify.MSTest/


## Snapshot management

Accepting or declining a snapshot file is part of the core workflow of Verify. There are several ways to do this and the approach(s) selected is a personal preference.

 * In the Windows Tray via [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)
 * [ReSharper test runner support](https://plugins.jetbrains.com/plugin/17241-verify-support) ([Source](https://github.com/matkoch/resharper-verify))
 * [Rider test runner support](https://plugins.jetbrains.com/plugin/17240-verify-support) ([Source](https://github.com/matkoch/resharper-verify))
 * [Via the clipboard](/docs/clipboard.md).
 * Manually making the change in the [launched diff tool](https://github.com/VerifyTests/DiffEngine#supported-tools). Either with a copy paste, or some tools have commands to automate this via a shortcut or a button.
 * Manually on the file system. By renaming the `.received.` file to `.verified.`. This can be automated via a scripted to bulk accept all (by matching a pattern) `.received.` files.


## Usage

**All examples use [ImplicitUsings](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#implicitusings). Ensure the following is set to have examples compile correctly `<ImplicitUsings>enable</ImplicitUsings>`**


### Class being tested

Given a class to be tested:

snippet: ClassBeingTested


### xUnit

Support for [xUnit](https://xunit.net/)

snippet: SampleTestXunit

 * [Verify Xunit Intro](https://www.youtube.com/watch?v=uGVogEltSkY)


### NUnit

Support for [NUnit](https://nunit.org/)

snippet: SampleTestNUnit


### Expecto

Support for [Expecto](https://github.com/haf/expecto)

snippet: SampleTestExpecto


#### Caveats

Due to the nature of the Expecto implementation, the following APIs in Verify are not supported.

 * `settings.UseTypeName()`
 * `settings.UseMethodName()`
 * `settings.UseParameters()`
 * `settings.UseTextForParameters()`

Instead use a custom `name` parameter.


### MSTest

Support for [MSTest](https://github.com/Microsoft/testfx-docs)

snippet: SampleTestMSTest


### Initial Verification

When the test is initially run will fail. If a [Diff Tool](https://github.com/VerifyTests/DiffEngine) is detected it will display the diff.

![InitialDiff](/docs/InitialDiff.png)

To verify the result:

 * Execute the command from the [Clipboard](/docs/clipboard.md), or
 * Accept with [DiffEngineTray tool](https://github.com/VerifyTests/DiffEngine/blob/master/docs/tray.md),
 * Accept with [ReSharper Addin](https://plugins.jetbrains.com/plugin/17241-verify-support) or [Rider Addin](https://plugins.jetbrains.com/plugin/17240-verify-support)
 * Use the diff tool to accept the changes, or
 * Manually copy the text to the new file


#### Verified result

This will result in the `Sample.Test.verified.txt` being created:

snippet: Verify.Xunit.Tests/Snippets/Sample.Test.verified.txt


### Subsequent Verification

If the implementation of `ClassBeingTested` changes:

snippet: ClassBeingTestedChanged

And the test is re run it will fail.


#### The [Diff Tool](https://github.com/VerifyTests/DiffEngine) will display the diff:

![SecondDiff](/docs/SecondDiff.png)

The same approach can be used to verify the results and the change to `Sample.Test.verified.txt` is committed to source control along with the change to `ClassBeingTested`.


### VerifyJson

`VerifyJson` performs the following actions

 * Convert to `JToken` (if necessary).
 * Apply [ignore member by name](/docs/serializer-settings.md#ignore-member-by-name) for keys.
 * PrettyPrint the resulting text.

snippet: VerifyJson

Results in:

snippet: SerializationTests.VerifyJsonString.verified.txt


## Received and Verified

 * **All `*.verified.*` files should be committed to source control.**
 * **All `*.received.*` files should be excluded from source control.**


## Static settings

Most settings are available at the both global level and at the instance level.

When modifying settings at the both global level it should be done using a Module Initializer:

snippet: StaticSettings.cs


## Versioning

Verify follows [Semantic Versioning](https://semver.org/). The same applies for [extensions to Verify](#extensions). Small changes in the resulting snapshot files may be deployed in a minor version. As such nuget updates to `Verify.*` should be done as follows:

 * Updates all `Verify.*`packages in isolation
 * Re-run all tests.
 * If there are changes, ensure they look correct given the release notes. If the changes do not look correct, raise an issue.
 * Accept those changes.

Snapshot changes do not trigger a major version change to avoid causing [Diamond dependency](https://en.wikipedia.org/wiki/Dependency_hell#Problems) issues for downstream extensions.


## Media

 * [Verify Xunit Intro (26 Apr 2020)](https://www.youtube.com/watch?v=uGVogEltSkY)
 * [OSS Power-Ups: Verify (14 Jul 2021)](https://www.youtube.com/watch?v=ZD5-51iCmU0)
 * [Unhandled Exception podcast: Snapshot Testing (26 Nov 2021)](https://unhandledexceptionpodcast.com/posts/0029-snapshottesting/)
 * [Testing an incremental generator with snapshot testing (14 Dec 2021)](https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/)
 * [5 helpful Nuget package for Unit Testing in .NET (16 Oct 2021)](https://medium.com/@niteshsinghal85/5-helpful-nuget-package-for-unit-testing-in-net-87c2e087c6d)
 * [5 open source .NET projects that deserve more attention (9 Sep 2021)](https://www.youtube.com/watch?v=mwHWPoKEmyY&t=515s)


## Extensions

 * [Verify.AngleSharp](https://github.com/VerifyTests/Verify.AngleSharp): Html verification utilities via [AngleSharp](https://github.com/AngleSharp/AngleSharp).
 * [Verify.AspNetCore](https://github.com/VerifyTests/Verify.AspNetCore): Verification of AspNetCore bits.
 * [Verify.Aspose](https://github.com/VerifyTests/Verify.Aspose): Verification of documents (pdf, docx, xslx, and pptx) via Aspose.
 * [Verify.Blazor](https://github.com/VerifyTests/Verify.Blazor): Verification of [Blazor Component](https://docs.microsoft.com/en-us/aspnet/core/blazor/#components) via [bunit](https://bunit.egilhansen.com) or via raw Blazor rendering.
 * [Verify.Cosmos](https://github.com/VerifyTests/Verify.Cosmos): Verification of [Azure CosmosDB](https://docs.microsoft.com/en-us/azure/cosmos-db/).
 * [Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex): Comparison of text via [DiffPlex](https://github.com/mmanela/diffplex).
 * [Verify.DocNet](https://github.com/VerifyTests/Verify.DocNet): Verification of pdfs via [DocNet](https://github.com/GowenGit/docnet).
 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework): Verification of EntityFramework bits.
 * [Verify.GrapeCity](https://github.com/VerifyTests/Verify.GrapeCity): Verification of documents (pdf, docx, and xslx) via [GrapeCity Documents API](https://www.grapecity.com/documents-api). 
 * [Verify.HeadlessBrowsers](https://github.com/VerifyTests/Verify.HeadlessBrowsers): Verification of Web UIs using [Playwright](https://github.com/microsoft/playwright-sharp), [Puppeteer Sharp](https://github.com/hardkoded/puppeteer-sharp), or [Selenium](https://www.selenium.dev).
 * [Verify.Http](https://github.com/VerifyTests/Verify.Http): Verification of Http bits.
 * [Verify.ICSharpCode.Decompiler](https://github.com/VerifyTests/Verify.ICSharpCode.Decompiler): Comparison of assemblies and types via [ICSharpCode.Decompiler](https://github.com/icsharpcode/ILSpy/wiki/Getting-Started-With-ICSharpCode.Decompiler).
 * [Verify.ImageHash](https://github.com/VerifyTests/Verify.ImageHash): Comparison of images via [ImageHash](https://github.com/coenm/ImageHash).
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp](https://github.com/VerifyTests/Verify.ImageSharp): Verification of images via [ImageSharp](https://github.com/SixLabors/ImageSharp).
 * [Verify.NServiceBus](https://github.com/VerifyTests/Verify.NServiceBus): Verify NServiceBus Test Contexts.
 * [Verify.MicrosoftLogging](https://github.com/VerifyTests/Verify.MicrosoftLogging): Verify MicrosoftLogging.
 * [Verify.MongoDB](https://github.com/flcdrg/Verify.MongoDB): Verification of [MongoDB](https://www.mongodb.com/) bits.
 * [Verify.NodaTime](https://github.com/VerifyTests/Verify.NodaTime): Support for [NodaTime](https://nodatime.org/).
 * [Verify.NSubstitute](https://github.com/VerifyTests/Verify.NSubstitute): Support for [NSubstitute](https://nsubstitute.github.io/) types
 * [Verify.Phash](https://github.com/VerifyTests/Verify.Phash): Comparison of images via [Phash](https://github.com/pgrho/phash).
 * [Verify.Quibble](https://github.com/VerifyTests/Verify.Quibble): Comparison of objects via [Quibble](https://github.com/nrkno/Quibble).
 * [Verify.RavenDb](https://github.com/VerifyTests/Verify.RavenDb): Verification of [RavenDb](https://ravendb.net) bits.
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer): Verification of SqlServer bits.
 * [Verify.SourceGenerators](https://github.com/VerifyTests/Verify.SourceGenerators): Verification of C# Source Generators.
 * [Verify.WinForms](https://github.com/VerifyTests/Verify.WinForms): Verification of WinForms UIs.
 * [Verify.Xamarin](https://github.com/VerifyTests/Verify.Xamarin): Verification of Xamarin UIs.
 * [Verify.Xaml](https://github.com/VerifyTests/Verify.Xaml): Verification of Xaml UIs.
 * [Spectre.Verify.Extensions](https://github.com/spectresystems/spectre.verify.extensions): Add an attribute driven file naming convention to Verify.
 * [Verify.Syncfusion](https://github.com/VerifyTests/Verify.Syncfusion): Verification of documents (pdf, docx, xslx, and pptx) via [Syncfusion File Formats](https://help.syncfusion.com/file-formats/introduction).


## More Documentation

include: doc-index


## Alternatives

Projects/tools that may be a better alternative to Verify

 * [ApprovalTests](https://github.com/approvals/ApprovalTests.Net)
 * [Snapshooter](https://github.com/SwissLife-OSS/Snapshooter)
 * [Snapper](https://github.com/theramis/Snapper)
 * [Polaroider](https://github.com/WickedFlame/Polaroider)


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).
