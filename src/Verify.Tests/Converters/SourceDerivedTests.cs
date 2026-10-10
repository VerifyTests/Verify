// A conversion that tells its source from what it derived from it: how the targets are named, what
// is converted again, how they are compared and what an exclusion applies to. What reaches a diff
// tool is in StaticSettingsTests, since that is observed through a static seam.

using System.Security;

public class SourceDerivedTests
{
    [ModuleInitializer]
    public static void Init()
    {
        // The shape the document plugins have: the document, and a page of it drawn and read
        VerifierSettings.RegisterStreamConverter("sddoc", (_, stream, _) => Document("sddoc", stream));

        // A container. Each attachment is a document, converted in its turn
        VerifierSettings.RegisterStreamConverter(
            "sdmail",
            (_, stream, _) =>
                new(
                    null,
                    new Target("sdmail", stream),
                    [
                        new("sddoc", Stream("first attachment"), "Attachment1"),
                        new("sddoc", Stream("second attachment"), "Attachment2")
                    ]));

        // Gives back its document in a newer format, one that has a converter of its own
        VerifierSettings.RegisterStreamConverter(
            "sdlegacy",
            (_, _, _) =>
                new(
                    null,
                    new Target("sdmodern", Stream("the document, saved as the newer format")),
                    [new("txt", "derived from the legacy document", "text")]));

        // The same, by a converter that does not tell a source from what it derived
        VerifierSettings.RegisterStreamConverter(
            "sdouter",
            (_, _, _) =>
                new(
                    null,
                    [new("sdmodern", Stream("not for conversion"), null, performConversion: false)]));

        VerifierSettings.RegisterStreamConverter("sdmodern", NeverConverted);

        // A document and a page of it that draws the same whatever the document holds, so that
        // the page differs only when its verified file says so
        VerifierSettings.RegisterStreamConverter(
            "sdbypass",
            (_, stream, _) =>
            {
                var content = new MemoryStream();
                stream.CopyTo(content);
                return new(
                    null,
                    new Target("sdbypass", content),
                    [new("sdpage", Stream("the page, drawn"), "page_0001")]);
            });

        VerifierSettings.RegisterStreamConverter(
            "sdcheck",
            (_, stream, context) =>
            {
                renderedPage = !context.IsDerivedTargetExcluded("sdpage");
                List<Target> derived = [];
                if (renderedPage)
                {
                    derived.Add(new("sdpage", Stream("the page, drawn"), "page_0001"));
                }

                return new(null, new Target("sdcheck", stream), derived);
            });

        // Asks before producing anything, so with everything excluded it returns nothing at all
        VerifierSettings.RegisterStreamConverter(
            "sdskip",
            (_, _, context) =>
            {
                Target? source = null;
                if (!context.IsTargetExcluded("sdskip"))
                {
                    source = new("sdskip", Stream("the document"));
                }

                List<Target> derived = [];
                if (!context.IsDerivedTargetExcluded("sdpage"))
                {
                    derived.Add(new("sdpage", Stream("the page, drawn"), "page_0001"));
                }

                return new(null, source, derived);
            });

        // Derives a target that a converter of the older kind then converts in its turn
        VerifierSettings.RegisterStreamConverter(
            "sdchain",
            (_, _, _) => new(null, null, [new("sdlink", Stream("to be converted again"), "link")]));
        VerifierSettings.RegisterStreamConverter(
            "sdlink",
            (name, _, _) => new(null, [new("sdpage", Stream("the page, drawn"), name)]));

        VerifierSettings.RegisterFileConverter<TypedDocument>(
            (document, _) =>
                new(
                    "the typed info",
                    new Target("sddoc", Stream(document.Content)),
                    [new("sdpage", Stream($"page of {document.Content}"), "page_0001")]));
    }

    static bool renderedPage;

    static ConversionResult NeverConverted(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        throw new("A source, and a target that opted out, are not converted again.");

    record TypedDocument(string Content);

    static ConversionResult Document(string extension, Stream stream)
    {
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        return new(
            $"info of {content}",
            new Target(extension, Stream(content)),
            [
                new("sdpage", Stream($"page of {content}"), "page_0001"),
                new("txt", $"text of {content}", "page_0001")
            ]);
    }

    static MemoryStream Stream(string content) =>
        new(Encoding.UTF8.GetBytes(content));

    [Fact]
    public async Task ADerivedTargetKeepsTheNameItsConverterGaveIt()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Stream("the document"), "sddoc")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.sdpage",
                "Test#page_0001.verified.txt",
                "Test.verified.sddoc",
                "Test.verified.txt"
            ],
            Names(result, nameof(ADerivedTargetKeepsTheNameItsConverterGaveIt)));
    }

    /// <summary>
    /// The converter of an attachment is passed the attachment's name and ignores it. Its document
    /// takes that name, what it derived is named after it, and so is its info.
    /// </summary>
    [Fact]
    public async Task NamesAreRelativeToTheTargetThatWasConverted()
    {
        using var temp = new TempDirectory();
        List<Target> attachments =
        [
            new("sddoc", Stream("first"), "Attachment1"),
            new("sddoc", Stream("second"), "Attachment2")
        ];

        var result = await Verify(attachments)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#Attachment1.page_0001.verified.sdpage",
                "Test#Attachment1.page_0001.verified.txt",
                "Test#Attachment1.verified.sddoc",
                "Test#Attachment1.verified.txt",
                "Test#Attachment2.page_0001.verified.sdpage",
                "Test#Attachment2.page_0001.verified.txt",
                "Test#Attachment2.verified.sddoc",
                "Test#Attachment2.verified.txt"
            ],
            Names(result, nameof(NamesAreRelativeToTheTargetThatWasConverted)));
    }

    /// <summary>
    /// The same through a container that is itself converted, where the infos of every conversion
    /// are gathered into the one info of the verification.
    /// </summary>
    [Fact]
    public async Task ADocumentInsideAContainerIsNamedAfterItsPlaceInIt()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Stream("the mail"), "sdmail")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#Attachment1.page_0001.verified.sdpage",
                "Test#Attachment1.page_0001.verified.txt",
                "Test#Attachment1.verified.sddoc",
                "Test#Attachment2.page_0001.verified.sdpage",
                "Test#Attachment2.page_0001.verified.txt",
                "Test#Attachment2.verified.sddoc",
                "Test.verified.sdmail",
                "Test.verified.txt"
            ],
            Names(result, nameof(ADocumentInsideAContainerIsNamedAfterItsPlaceInIt)));
    }

    /// <summary>
    /// A document given back in another format is the document, not something to convert: the
    /// converter registered for that format throws.
    /// </summary>
    [Fact]
    public async Task ASourceIsNotConvertedAgain()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Stream("the legacy document"), "sdlegacy")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#text.verified.txt",
                "Test.verified.sdmodern"
            ],
            Names(result, nameof(ASourceIsNotConvertedAgain)));
    }

    /// <summary>
    /// What a target passed to a verification could always ask for, and a target a converter
    /// returned could not: the flag was not read for those.
    /// </summary>
    [Fact]
    public async Task ATargetAConverterReturnedCanOptOutOfConversion()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Stream("the outer document"), "sdouter")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            ["Test.verified.sdmodern"],
            Names(result, nameof(ATargetAConverterReturnedCanOptOutOfConversion)));
    }

    [Fact]
    public async Task ATypedConverterNamesItsSourceTheSameWay()
    {
        using var temp = new TempDirectory();

        // The source is a format with a stream converter of its own, which would add a text file
        // of the page were it run
        var result = await Verify(new TypedDocument("typed"))
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.sdpage",
                "Test.verified.sddoc",
                "Test.verified.txt"
            ],
            Names(result, nameof(ATypedConverterNamesItsSourceTheSameWay)));
    }

    static int maskedCount;

    // A comparer that masks every difference by always reporting equal
    static Task<CompareResult> Masking(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        Interlocked.Increment(ref maskedCount);
        return Task.FromResult(CompareResult.Equal);
    }

    /// <summary>
    /// With no flag set by the converter. A page of a document that has not changed is given to
    /// its comparer, and a page of one that has is compared exactly.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="SecurityException"></exception>
    [Fact]
    public async Task ADifferingSourceHasItsDerivedTargetsComparedExactly()
    {
        using var temp = new TempDirectory();
        var prefix = Path.Combine(temp, $"{nameof(SourceDerivedTests)}.{nameof(ADifferingSourceHasItsDerivedTargetsComparedExactly)}");
        await File.WriteAllTextAsync($"{prefix}.verified.sdbypass", "the document");
        await File.WriteAllTextAsync($"{prefix}#page_0001.verified.sdpage", "a page the comparer would let through");

        maskedCount = 0;
        var equal = await Verify(Stream("the document"), "sdbypass")
            .UseDirectory(temp)
            .UseStreamComparer(Masking, "sdpage")
            .DisableRequireUniquePrefix()
            .DisableDiff();
        Assert.Equal(1, maskedCount);
        Assert.Equal(2, equal.Files.Count());

        // Nothing but the document changed, and no text target failed ahead of the page
        maskedCount = 0;
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(Stream("the document, changed"), "sdbypass")
                .UseDirectory(temp)
                .UseStreamComparer(Masking, "sdpage")
                .DisableRequireUniquePrefix()
                .DisableDiff());
        Assert.Equal(0, maskedCount);
        Assert.Contains("#page_0001", exception.Message);
    }

    /// <summary>
    /// A text target that fails makes the binary targets after it skip their comparers, which was
    /// the only sign there was that a document had changed. With the document itself compared,
    /// and equal, a change to its info says nothing of its pages.
    /// </summary>
    [Fact]
    public async Task AnInfoThatDiffersLeavesThePagesOfAnUnchangedSourceToTheirComparers()
    {
        using var temp = new TempDirectory();
        var prefix = Path.Combine(temp, $"{nameof(SourceDerivedTests)}.{nameof(AnInfoThatDiffersLeavesThePagesOfAnUnchangedSourceToTheirComparers)}");
        await File.WriteAllTextAsync($"{prefix}.verified.txt", "an info in the shape it used to have");
        await File.WriteAllTextAsync($"{prefix}.verified.sddoc", "the document");
        await File.WriteAllTextAsync($"{prefix}#page_0001.verified.sdpage", "a page the comparer would let through");
        await File.WriteAllTextAsync($"{prefix}#page_0001.verified.txt", "text of the document");

        maskedCount = 0;
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(Stream("the document"), "sddoc")
                .UseDirectory(temp)
                .UseStreamComparer(Masking, "sdpage")
                .DisableDiff());

        Assert.Equal(1, maskedCount);
        var notEqual = NotEqual(exception.Message);
        Assert.Contains(".verified.txt", notEqual);
        Assert.DoesNotContain("#page_0001", notEqual);
    }

    /// <summary>
    /// One document differing says nothing of the pages of another, which the flag this replaces
    /// could not express: it switched the comparers off for every target that followed.
    /// </summary>
    [Fact]
    public async Task ADifferingSourceLeavesTheTargetsOfAnotherToTheirComparers()
    {
        using var temp = new TempDirectory();
        var prefix = Path.Combine(temp, $"{nameof(SourceDerivedTests)}.{nameof(ADifferingSourceLeavesTheTargetsOfAnotherToTheirComparers)}");
        foreach (var attachment in new[] {"Attachment1", "Attachment2"})
        {
            await File.WriteAllTextAsync($"{prefix}#{attachment}.verified.sdbypass", attachment);
            await File.WriteAllTextAsync($"{prefix}#{attachment}.page_0001.verified.sdpage", "a page the comparer would let through");
        }

        List<Target> attachments =
        [
            new("sdbypass", Stream("Attachment1, changed"), "Attachment1"),
            new("sdbypass", Stream("Attachment2"), "Attachment2")
        ];

        maskedCount = 0;
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(attachments)
                .UseDirectory(temp)
                .UseStreamComparer(Masking, "sdpage")
                .DisableDiff());

        // The second attachment's page went to the comparer, and only that one
        Assert.Equal(1, maskedCount);
        Assert.Contains("#Attachment1.page_0001", exception.Message);
        Assert.DoesNotContain("#Attachment2.page_0001", NotEqual(exception.Message));
    }

    /// <summary>
    /// The info is the first target and the document the second, so going through them in order
    /// compared the info before anything was known of the document.
    /// </summary>
    [Fact]
    public async Task TheInfoOfADifferingSourceIsComparedExactlyToo()
    {
        using var temp = new TempDirectory();
        var prefix = Path.Combine(temp, $"{nameof(SourceDerivedTests)}.{nameof(TheInfoOfADifferingSourceIsComparedExactlyToo)}");
        await File.WriteAllTextAsync($"{prefix}.verified.txt", "an info the comparer would let through");
        await File.WriteAllTextAsync($"{prefix}.verified.sddoc", "the document");
        await File.WriteAllTextAsync($"{prefix}#page_0001.verified.sdpage", "page of the document, changed");
        await File.WriteAllTextAsync($"{prefix}#page_0001.verified.txt", "text of the document, changed");

        var compared = 0;
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(Stream("the document, changed"), "sddoc")
                .UseDirectory(temp)
                .UseStringComparer(
                    (_, _, _) =>
                    {
                        compared++;
                        return Task.FromResult(CompareResult.Equal);
                    })
                .DisableDiff());

        Assert.Equal(0, compared);
        Assert.Contains("TheInfoOfADifferingSourceIsComparedExactlyToo.verified.txt", NotEqual(exception.Message));
    }

    [Fact]
    public async Task ADerivedTargetCanBeExcluded()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Stream("the document"), "sddoc")
            .UseDirectory(temp)
            .ExcludeDerivedTargets("sdpage")
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.txt",
                "Test.verified.sddoc",
                "Test.verified.txt"
            ],
            Names(result, nameof(ADerivedTargetCanBeExcluded)));
    }

    /// <summary>
    /// Which is the difference from ExcludeTargets: the extension is only excluded where a
    /// converter derived the target. The document, the info and a target passed in stay.
    /// </summary>
    [Fact]
    public async Task ExcludingADerivedExtensionLeavesTheSameExtensionElsewhere()
    {
        using var temp = new TempDirectory();
        List<Target> targets = [new("sdpage", Stream("a page passed in as itself"))];

        var passed = await Verify(targets)
            .UseDirectory(temp)
            .ExcludeDerivedTargets("sdpage")
            .AutoVerify()
            .DisableDiff();
        Assert.Equal(
            ["Test.verified.sdpage"],
            Names(passed, nameof(ExcludingADerivedExtensionLeavesTheSameExtensionElsewhere)));

        var document = await Verify(Stream("the document"), "sddoc")
            .UseDirectory(temp)
            .ExcludeDerivedTargets("sddoc", "txt")
            .DisableRequireUniquePrefix()
            .AutoVerify()
            .DisableDiff();
        Assert.Contains(
            "Test.verified.sddoc",
            Names(document, nameof(ExcludingADerivedExtensionLeavesTheSameExtensionElsewhere)));
        Assert.Contains(
            "Test.verified.txt",
            Names(document, nameof(ExcludingADerivedExtensionLeavesTheSameExtensionElsewhere)));
        Assert.DoesNotContain(
            "Test#page_0001.verified.txt",
            Names(document, nameof(ExcludingADerivedExtensionLeavesTheSameExtensionElsewhere)));
    }

    /// <summary>
    /// The converter observes the exclusion and never draws the page, for either way of excluding it.
    /// </summary>
    [Fact]
    public async Task AConverterCanAskWhetherADerivedTargetIsExcluded()
    {
        using var temp = new TempDirectory();

        renderedPage = false;
        await Verify(Stream("the document"), "sdcheck")
            .UseDirectory(temp)
            .DisableRequireUniquePrefix()
            .AutoVerify()
            .DisableDiff();
        Assert.True(renderedPage);

        await Verify(Stream("the document"), "sdcheck")
            .UseDirectory(temp)
            .ExcludeDerivedTargets("sdpage")
            .DisableRequireUniquePrefix()
            .AutoVerify()
            .DisableDiff();
        Assert.False(renderedPage);

        renderedPage = true;
        await Verify(Stream("the document"), "sdcheck")
            .UseDirectory(temp)
            .ExcludeTargets("sdpage")
            .DisableRequireUniquePrefix()
            .AutoVerify()
            .DisableDiff();
        Assert.False(renderedPage);
    }

    [Fact]
    public async Task ExcludingEveryTargetThrows()
    {
        var exception = await Assert.ThrowsAsync<Exception>(
            () => Verify(Stream("the document"), "sdcheck")
                .ExcludeTargets("sdcheck")
                .ExcludeDerivedTargets("sdpage")
                .DisableDiff());
        Assert.Contains("All targets have been excluded", exception.Message);
    }

    /// <summary>
    /// A converter that is told what is excluded produces none of it, so there is nothing left
    /// for the exclusion to remove, and nothing to say the verification verified nothing.
    /// </summary>
    [Fact]
    public async Task ExcludingEveryTargetThrowsWhenTheConverterLeftThemOutItself()
    {
        var exception = await Assert.ThrowsAsync<Exception>(
            () => Verify(Stream("the document"), "sdskip")
                .ExcludeTargets("sdskip")
                .ExcludeDerivedTargets("sdpage")
                .DisableDiff());
        Assert.Contains("All targets have been excluded", exception.Message);
    }

    /// <summary>
    /// What a converter of the older kind makes of a derived target is derived too, including
    /// where the document it all came from was left out.
    /// </summary>
    [Fact]
    public async Task WhatIsConvertedFromADerivedTargetIsDerived()
    {
        using var temp = new TempDirectory();

        var kept = await Verify(Stream("the document"), "sdchain")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();
        Assert.Equal(
            ["Test#link.verified.sdpage"],
            Names(kept, nameof(WhatIsConvertedFromADerivedTargetIsDerived)));

        var exception = await Assert.ThrowsAsync<Exception>(
            () => Verify(Stream("the document"), "sdchain")
                .UseDirectory(temp)
                .ExcludeDerivedTargets("sdpage")
                .DisableRequireUniquePrefix()
                .DisableDiff());
        Assert.Contains("All targets have been excluded", exception.Message);
    }

    [Fact]
    public void NoExtensionsThrows() =>
        Assert.Throws<ArgumentException>(() => new VerifySettings().ExcludeDerivedTargets());

    // The verified file names of a result, less the test they belong to, in a fixed order
    static List<string> Names(VerifyResult result, string method) =>
        result.Files
            .Select(Path.GetFileName)
            .Select(_ => _!.Replace($"{nameof(SourceDerivedTests)}.{method}", "Test"))
            .OrderBy(_ => _, StringComparer.Ordinal)
            .ToList();

    // The part of an exception message that lists the files that differed
    static string NotEqual(string message)
    {
        var start = message.IndexOf("NotEqual:", StringComparison.Ordinal);
        if (start < 0)
        {
            return "";
        }

        var end = message.Length;
        foreach (var section in new[] {"\nDelete:", "\nEqual:", "\nFileContent:"})
        {
            var index = message.IndexOf(section, start, StringComparison.Ordinal);
            if (index >= 0 &&
                index < end)
            {
                end = index;
            }
        }

        return message.Substring(start, end - start);
    }
}
