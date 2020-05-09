<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /readme.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# <img src="/src/icon.png" height="30px"> Verify

[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Xunit.svg?label=Verify.Xunit)](https://www.nuget.org/packages/Verify.Xunit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.NUnit.svg?label=Verify.NUnit)](https://www.nuget.org/packages/Verify.NUnit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.MSTest.svg?label=Verify.MSTest)](https://www.nuget.org/packages/Verify.MSTest/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Bunit.svg?label=Verify.Bunit)](https://www.nuget.org/packages/Verify.Bunit/)

Verification tool to enable simple approval of complex models and documents.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<!-- toc -->
## Contents

  * [Verification versus Assertion](#verification-versus-assertion)
  * [Usage](#usage)
    * [Class being tested](#class-being-tested)
    * [xUnit](#xunit)
    * [NUnit](#nunit)
    * [MSTest](#mstest)
    * [bunit](#bunit)
    * [Initial Verification](#initial-verification)
    * [Subsequent Verification](#subsequent-verification)
    * [Disable Clipboard](#disable-clipboard)
    * [AutoVerify](#autoverify)
    * [OnHandlers](#onhandlers)
  * [Received and Verified](#received-and-verified)
  * [Not valid json](#not-valid-json)
  * [Videos](#videos)
  * [Extensions](#extensions)
  * [Alternatives](#alternatives)
  * [Security contact information](#security-contact-information)<!-- endtoc -->
  * [Serializer Settings](/docs/serializer-settings.md) <!-- include: doc-index. path: /docs/mdsource/doc-index.include.md -->
  * [File naming](/docs/naming.md)
  * [Parameterised tests](/docs/parameterised.md)
  * [Named Tuples](/docs/named-tuples.md)
  * [Scrubbers](/docs/scrubbers.md)
  * [Diff Engine](https://github.com/VerifyTests/DiffEngine)
  * [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/master/docs/diff-tool.md)
  * [Diff Tool Order](https://github.com/VerifyTests/DiffEngine/blob/master/docs/diff-tool.order.md)
  * [Custom Diff Tool](https://github.com/VerifyTests/DiffEngine/blob/master/docs/diff-tool.custom.md)
  * [Using anonymous types](/docs/anonymous-types.md)
  * [Verifying binary data](/docs/binary.md)
  * [Comparers](/docs/comparer.md)
  * [Compared to ApprovalTests](/docs/compared-to-approvaltests.md) <!-- end include: doc-index. path: /docs/mdsource/doc-index.include.md -->


## NuGet packages

 * https://nuget.org/packages/Verify.Xunit/
 * https://nuget.org/packages/Verify.NUnit/
 * https://nuget.org/packages/Verify.MSTest/
 * https://nuget.org/packages/Verify.Bunit/


## Verification versus Assertion

Given the following method:


#### Class being tested

<!-- snippet: ClassBeingTested -->
<a id='snippet-classbeingtested'/></a>
```cs
public static class ClassBeingTested
{
    public static Person FindPerson()
    {
        return new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string>
            {
                "Sam",
                "Mary"
            },
            Address = new Address
            {
                Street = "4 Puddle Lane",
                Country = "USA"
            }
        };
    }
}
```
<sup><a href='/src/TargetLibrary/ClassBeingTested.cs#L4-L29' title='File snippet `classbeingtested` was extracted from'>snippet source</a> | <a href='#snippet-classbeingtested' title='Navigate to start of snippet `classbeingtested`'>anchor</a></sup>
<!-- endsnippet -->


#### Tests

Compare a traditional assertion based test to a verification test.


##### Traditional assertion test:

<!-- snippet: TraditionalTest -->
<a id='snippet-traditionaltest'/></a>
```cs
[Fact]
public void TraditionalTest()
{
    var person = ClassBeingTested.FindPerson();
    Assert.Equal(new Guid("ebced679-45d3-4653-8791-3d969c4a986c"), person.Id);
    Assert.Equal(Title.Mr, person.Title);
    Assert.Equal("John", person.GivenNames);
    Assert.Equal("Smith", person.FamilyName);
    Assert.Equal("Jill", person.Spouse);
    Assert.Equal(2, person.Children.Count);
    Assert.Equal("Sam", person.Children[0]);
    Assert.Equal("Mary", person.Children[1]);
    Assert.Equal("4 Puddle Lane", person.Address.Street);
    Assert.Equal("USA", person.Address.Country);
}
```
<sup><a href='/src/Verify.Xunit.Tests/Snippets/CompareToAssert.cs#L10-L26' title='File snippet `traditionaltest` was extracted from'>snippet source</a> | <a href='#snippet-traditionaltest' title='Navigate to start of snippet `traditionaltest`'>anchor</a></sup>
<!-- endsnippet -->


##### Verification test

<!-- snippet: VerificationTest -->
<a id='snippet-verificationtest'/></a>
```cs
[Fact]
public Task Simple()
{
    var person = ClassBeingTested.FindPerson();
    return Verify(person);
}
```
<sup><a href='/src/Verify.Xunit.Tests/Snippets/CompareToAssert.cs#L28-L35' title='File snippet `verificationtest` was extracted from'>snippet source</a> | <a href='#snippet-verificationtest' title='Navigate to start of snippet `verificationtest`'>anchor</a></sup>
<!-- endsnippet -->


#### Comparing Verification to Assertion

  * **Less test code**: verification test require less code to write.
  * **Reduced risk of incorrect test code**: Given the above assertion based test it would be difficult to ensure that no property is missing from the assertion. For example if a new property is added to the model. In the verification test that change would automatically be highlighted when the test is next run.
  * **Test failure visualization**: Verification test allows [visualization in a diff tool](https://github.com/VerifyTests/DiffEngine) that works for [complex models](/docs/SecondDiff.png) and [binary documents](/docs/binary.md).
  * **Multiple changes visualized in singe test run**: In the assertion approach, if multiple assertions require changing, this only becomes apparent over multiple test runs. In the verification approach, multiple changes can be [visualized in one test run](/docs/SecondDiff.png).
  * **Simpler creation of test "contract"**: In the assertion approach, complex models can require significant code to do the initial assertion. In the verification approach, the actual test and code-under-test can be used to create that "contract". See [initial verification](#initial-verification).
  * **Verification files committed to source control**: All resulting verified files are committed to source control in the most appropriate format. This means these files can be viewed at any time using any tooling. The files can also be diff'd over the history of the code base. This works for any file type, for example:
    * Html content can be committed as `.html` files.
    * Office documents can be committed as a rendered `.png` (see [Verify.Aspose](https://github.com/VerifyTests/Verify.Aspose)).
    * Database schema can be committed as `.sql` (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).


## Usage


### Class being tested

Given a class to be tested:

<!-- snippet: ClassBeingTested -->
<a id='snippet-classbeingtested'/></a>
```cs
public static class ClassBeingTested
{
    public static Person FindPerson()
    {
        return new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string>
            {
                "Sam",
                "Mary"
            },
            Address = new Address
            {
                Street = "4 Puddle Lane",
                Country = "USA"
            }
        };
    }
}
```
<sup><a href='/src/TargetLibrary/ClassBeingTested.cs#L4-L29' title='File snippet `classbeingtested` was extracted from'>snippet source</a> | <a href='#snippet-classbeingtested' title='Navigate to start of snippet `classbeingtested`'>anchor</a></sup>
<!-- endsnippet -->


### xUnit

Support for [xUnit](https://xunit.net/)

<!-- snippet: SampleTestXunit -->
<a id='snippet-sampletestxunit'/></a>
```cs
public class SampleTest :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }

    public SampleTest(ITestOutputHelper output) :
        base(output)
    {
    }
}
```
<sup><a href='/src/Verify.Xunit.Tests/Snippets/SampleTest.cs#L6-L22' title='File snippet `sampletestxunit` was extracted from'>snippet source</a> | <a href='#snippet-sampletestxunit' title='Navigate to start of snippet `sampletestxunit`'>anchor</a></sup>
<!-- endsnippet -->

 * [Verify Xunit Intro](https://www.youtube.com/watch?v=uGVogEltSkY)


### NUnit

Support for [NUnit](https://nunit.org/)

<!-- snippet: SampleTestNUnit -->
<a id='snippet-sampletestnunit'/></a>
```cs
[TestFixture]
public class SampleTest
{
    [Test]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }
}
```
<sup><a href='/src/Verify.NUnit.Tests/Snippets/SampleTest.cs#L5-L16' title='File snippet `sampletestnunit` was extracted from'>snippet source</a> | <a href='#snippet-sampletestnunit' title='Navigate to start of snippet `sampletestnunit`'>anchor</a></sup>
<!-- endsnippet -->


### MSTest

Support for [MSTest](https://github.com/Microsoft/testfx-docs)

<!-- snippet: SampleTestMSTest -->
<a id='snippet-sampletestmstest'/></a>
```cs
[TestClass]
public class SampleTest :
    VerifyBase
{
    [TestMethod]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
```
<sup><a href='/src/Verify.MSTest.Tests/Snippets/SampleTest.cs#L5-L17' title='File snippet `sampletestmstest` was extracted from'>snippet source</a> | <a href='#snippet-sampletestmstest' title='Navigate to start of snippet `sampletestmstest`'>anchor</a></sup>
<!-- endsnippet -->


### bunit

Support for rendering a [Blazor Component](https://docs.microsoft.com/en-us/aspnet/core/blazor/#components) to a verified file via [bunit](https://bunit.egilhansen.com/docs/).


#### Component test

Given the following Component:

<!-- snippet: TestComponent.razor -->
<a id='snippet-TestComponent.razor'/></a>
```razor
<div>
    <h1>@Title</h1>
    <button>MyButton</button>
</div>

@code {
    [Parameter]
    public string Title { get; set; } = "My Test Component";
}
```
<sup><a href='/src/Verify.Bunit.Tests/TestComponent.razor#L1-L9' title='File snippet `TestComponent.razor` was extracted from'>snippet source</a> | <a href='#snippet-TestComponent.razor' title='Navigate to start of snippet `TestComponent.razor`'>anchor</a></sup>
<!-- endsnippet -->

This test:

<!-- snippet: ComponentTest -->
<a id='snippet-componenttest'/></a>
```cs
[Fact]
public Task Component()
{
    var component = RenderComponent<TestComponent>();
    return Verify(component);
}
```
<sup><a href='/src/Verify.Bunit.Tests/VerifyObjectSamples.cs#L16-L24' title='File snippet `componenttest` was extracted from'>snippet source</a> | <a href='#snippet-componenttest' title='Navigate to start of snippet `componenttest`'>anchor</a></sup>
<!-- endsnippet -->

Will produce:

The component rendered as html `...Component.verified.html`:

<!-- snippet: VerifyObjectSamples.Component.verified.html -->
<a id='snippet-VerifyObjectSamples.Component.verified.html'/></a>
```html
<div>
    <h1>My Test Component</h1>
    <button>MyButton</button>
</div>
```
<sup><a href='/src/Verify.Bunit.Tests/VerifyObjectSamples.Component.verified.html#L1-L4' title='File snippet `VerifyObjectSamples.Component.verified.html` was extracted from'>snippet source</a> | <a href='#snippet-VerifyObjectSamples.Component.verified.html' title='Navigate to start of snippet `VerifyObjectSamples.Component.verified.html`'>anchor</a></sup>
<!-- endsnippet -->

And the current model rendered as txt `...Component.info.verified.txt`:

<!-- snippet: VerifyObjectSamples.Component.info.verified.txt -->
<a id='snippet-VerifyObjectSamples.Component.info.verified.txt'/></a>
```txt
{
  Instance: {
    Title: 'My Test Component'
  }
}
```
<sup><a href='/src/Verify.Bunit.Tests/VerifyObjectSamples.Component.info.verified.txt#L1-L5' title='File snippet `VerifyObjectSamples.Component.info.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-VerifyObjectSamples.Component.info.verified.txt' title='Navigate to start of snippet `VerifyObjectSamples.Component.info.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


### Initial Verification

When the test is initially run will fail with:

```
First verification. SampleTest.Simple.verified.txt not found.
Verification command has been copied to the clipboard.
```

The clipboard will contain the following:

> move /Y "C:\Code\Sample\SampleTest.Simple.received.txt" "C:\Code\Sample\SampleTest.Simple.verified.txt"

If a [Diff Tool](https://github.com/VerifyTests/DiffEngine) is detected it will display the diff:

![InitialDiff](/docs/InitialDiff.png)

To verify the result:

 * Execute the command from the clipboard, or
 * Use the diff tool to accept the changes, or
 * Manually copy the text to the new file

This will result in the `SampleTest.Simple.verified.txt` being created:

<!-- snippet: Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt -->
<a id='snippet-Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt'/></a>
```txt
{
  GivenNames: 'John',
  FamilyName: 'Smith',
  Spouse: 'Jill',
  Address: {
    Street: '4 Puddlde Lane',
    Country: 'USA'
  },
  Children: [
    'Sam',
    'Mary'
  ],
  Id: Guid_1
}
```
<sup><a href='/src/Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt#L1-L14' title='File snippet `Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt' title='Navigate to start of snippet `Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


### Subsequent Verification

If the implementation of `ClassBeingTested` changes:

<!-- snippet: ClassBeingTestedChanged -->
<a id='snippet-classbeingtestedchanged'/></a>
```cs
public static class ClassBeingTested
{
    public static Person FindPerson()
    {
        return new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            // Middle name added
            GivenNames = "John James",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string>
            {
                "Sam",
                "Mary"
            },
            Address = new Address
            {
                // Address changed
                Street = "64 Barnett Street",
                Country = "USA"
            }
        };
    }
}
```
<sup><a href='/src/TargetLibrary/ClassBeingTestedChanged.cs#L6-L33' title='File snippet `classbeingtestedchanged` was extracted from'>snippet source</a> | <a href='#snippet-classbeingtestedchanged' title='Navigate to start of snippet `classbeingtestedchanged`'>anchor</a></sup>
<!-- endsnippet -->

And the test is re run it will fail with

```
Verification command has been copied to the clipboard.
Assert.Equal() Failure
                                  ↓ (pos 21)
Expected: ···\n  GivenNames: 'John',\n  FamilyName: 'Smith',\n  Spouse: 'Jill···
Actual:   ···\n  GivenNames: 'John James',\n  FamilyName: 'Smith',\n  Spouse:···
                                  ↑ (pos 21)
```

The clipboard will again contain the following:

> move /Y "C:\Code\Sample\SampleTest.Simple.received.txt" "C:\Code\Sample\SampleTest.Simple.verified.txt"

And the [Diff Tool](https://github.com/VerifyTests/DiffEngine) is will display the diff:

![SecondDiff](/docs/SecondDiff.png)

The same approach can be used to verify the results and the change to `SampleTest.Simple.verified.txt` is committed to source control along with the change to `ClassBeingTested`.


### Disable Clipboard

The clipboard behavior can be disable using the following:


#### Per Test

<!-- snippet: DisableClipboard -->
<a id='snippet-disableclipboard'/></a>
```cs
var settings = new VerifySettings();
settings.DisableClipboard();
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L35-L40' title='File snippet `disableclipboard` was extracted from'>snippet source</a> | <a href='#snippet-disableclipboard' title='Navigate to start of snippet `disableclipboard`'>anchor</a></sup>
<!-- endsnippet -->


#### For all tests

<!-- snippet: DisableClipboardGlobal -->
<a id='snippet-disableclipboardglobal'/></a>
```cs
SharedVerifySettings.DisableClipboard();
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L55-L59' title='File snippet `disableclipboardglobal` was extracted from'>snippet source</a> | <a href='#snippet-disableclipboardglobal' title='Navigate to start of snippet `disableclipboardglobal`'>anchor</a></sup>
<!-- endsnippet -->

If clipboard is disabled for all tests, it can be re-enabled at the test level:

<!-- snippet: EnableClipboard -->
<a id='snippet-enableclipboard'/></a>
```cs
var settings = new VerifySettings();
settings.EnableClipboard();
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L45-L50' title='File snippet `enableclipboard` was extracted from'>snippet source</a> | <a href='#snippet-enableclipboard' title='Navigate to start of snippet `enableclipboard`'>anchor</a></sup>
<!-- endsnippet -->


#### For a machine

Set a `Verify.DisableClipboard` environment variable to `true`. This overrides the above settings.


### AutoVerify

In some scenarios it makes sense to auto-accept any changes as part of a given test run. For example:

 * Keeping a text representation of a Database schema in a `.verified.sql` file (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).

This can be done using `AutoVerify()`:

<!-- snippet: AutoVerify -->
<a id='snippet-autoverify'/></a>
```cs
var settings = new VerifySettings();
settings.AutoVerify();
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L64-L69' title='File snippet `autoverify` was extracted from'>snippet source</a> | <a href='#snippet-autoverify' title='Navigate to start of snippet `autoverify`'>anchor</a></sup>
<!-- endsnippet -->

Note that auto accepted changes in `.verified.` files remain visible in source control tooling.


### OnHandlers

`OnFirstVerify` is called when there is no verified file.

`OnVerifyMismatch` is called when a received file does not match the existing verified file.

<!-- snippet: OnHandlers -->
<a id='snippet-onhandlers'/></a>
```cs
public async Task OnHandlersSample()
{
    var settings = new VerifySettings();
    settings.OnFirstVerify(
        receivedFile =>
        {
            Debug.WriteLine(receivedFile);
            return Task.CompletedTask;
        });
    settings.OnVerifyMismatch(
        (receivedFile, verifiedFile, message) =>
        {
            Debug.WriteLine(receivedFile);
            Debug.WriteLine(verifiedFile);
            Debug.WriteLine(message);
            return Task.CompletedTask;
        });
    await Verify("value", settings);
}
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L11-L31' title='File snippet `onhandlers` was extracted from'>snippet source</a> | <a href='#snippet-onhandlers' title='Navigate to start of snippet `onhandlers`'>anchor</a></sup>
<!-- endsnippet -->


## Received and Verified

 * **All `*.verified.*` files should be committed to source control.**
 * **All `*.received.*` files should be excluded from source control.**


## Not valid json

Note that the output is technically not valid json. [Single quotes are used](docs/serializer-settings.md#single-quotes-used) and [names are not quoted](docs/serializer-settings.md#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


## Videos

 * [Verify Xunit Intro](https://www.youtube.com/watch?v=uGVogEltSkY)


## Extensions

 * [Verify.AngleSharp.Diffing](https://github.com/VerifyTests/Verify.AngleSharp.Diffing): Comparison of html files via [AngleSharp.Diffing](https://github.com/AngleSharp/AngleSharp.Diffing).
 * [Verify.Aspose](https://github.com/VerifyTests/VerifyTests.Aspose): Verification of documents (pdf, docx, xslx, and pptx) via Aspose.
 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework): Verification of EntityFramework bits.
 * [Verify.ICSharpCode.Decompiler](https://github.com/VerifyTests/Verify.ICSharpCode.Decompiler): Comparison of assemblies and types via [ICSharpCode.Decompiler](https://github.com/icsharpcode/ILSpy/wiki/Getting-Started-With-ICSharpCode.Decompiler).
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp](https://github.com/VerifyTests/Verify.ImageSharp): Verification of images via [ImageSharp](https://github.com/SixLabors/ImageSharp).
 * [Verify.NServiceBus](https://github.com/NServiceBusExtensions/Verify.NServiceBus): Verify NServiceBus Test Contexts.
 * [Verify.Phash](https://github.com/VerifyTests/Verify.Phash): Comparison of documents via [Phash](https://github.com/pgrho/phash).
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer): Verification of SqlServer bits.
 * [Verify.Web](https://github.com/VerifyTests/Verify.Web): Verification of web bits.
 * [Verify.WinForms](https://github.com/VerifyTests/Verify.WinForms): Verification of WinForms UIs.
 * [Verify.Xaml](https://github.com/VerifyTests/Verify.Xaml): Verification of Xaml UIs.


## Alternatives

Projects/tools that may be a better alternative to Verify

 * [ApprovalTests](https://github.com/approvals/ApprovalTests.Net)
 * [Snapshooter](https://github.com/SwissLife-OSS/Snapshooter)
 * [Snapper](https://github.com/theramis/Snapper)


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).
