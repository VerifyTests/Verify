using DiffEngine;

// Lives here, rather than in Verify.Tests, since what reaches a diff tool is observed through
// static seams, and this project runs serially with BaseTest resetting between tests.
//
// A converter that tells its document from what it derived from it has the derived files reported
// as such, so a tool drawing the document can show them beneath it and accept the lot as one. A
// file names its source only while that source is itself pending, and after the source has been
// reported.
public class SourceDerivedReportTests :
    BaseTest,
    IDisposable
{
    // Everything that would have gone to DiffEngine, in the order it would have gone
    List<string> reported = [];
    Func<FilePair, string?, Task> originalLaunchDiff = VerifyEngine.LaunchDiff;
    Func<string, string?, Task> originalAddDelete = RaisedDeletes.AddDelete;
    Action<string> originalSettleDelete = RaisedDeletes.SettleDelete;
    bool originalDisabled = DiffRunner.Disabled;
    TempDirectory temp = new();

    public SourceDerivedReportTests()
    {
        // All stood in for, so nothing reaches whatever tray or viewer is running on this machine
        VerifyEngine.LaunchDiff = (file, source) =>
        {
            reported.Add($"move {Path.GetFileName(file.ReceivedPath)}{From(source)}");
            return Task.CompletedTask;
        };
        RaisedDeletes.AddDelete = (file, source) =>
        {
            reported.Add($"delete {Path.GetFileName(file)}{From(source)}");
            return Task.CompletedTask;
        };
        RaisedDeletes.SettleDelete = _ =>
        {
        };

        // DiffEngine switches itself off on a build server, under continuous testing and under an
        // AI CLI, and nothing is launched while it is off
        DiffRunner.Disabled = false;
        BuildServerDetector.Detected = false;

        // The shape the document plugins have: the document, with a page of it drawn and read
        VerifierSettings.RegisterStreamConverter(
            "rdoc",
            (_, stream, _) =>
            {
                var content = Read(stream);
                return new(
                    $"info of {content}",
                    new Target("rdoc", Stream(content)),
                    [
                        new("rpage", Stream($"page of {content}"), "page_0001"),
                        new("txt", $"text of {content}", "page_0001")
                    ]);
            });

        // A document that is rendered to another, which is converted in its turn
        VerifierSettings.RegisterStreamConverter(
            "router",
            (_, stream, _) =>
            {
                var content = Read(stream);
                return new(
                    null,
                    new Target("router", Stream(content)),
                    [new("rdoc", Stream($"inner of {content}"))]);
            });
    }

    public void Dispose()
    {
        VerifyEngine.LaunchDiff = originalLaunchDiff;
        RaisedDeletes.AddDelete = originalAddDelete;
        RaisedDeletes.SettleDelete = originalSettleDelete;
        DiffRunner.Disabled = originalDisabled;
        temp.Dispose();
    }

    /// <summary>
    /// The info is the first target and the document the second, and the document is still the
    /// first thing a diff tool hears of.
    /// </summary>
    [Fact]
    public async Task ASourceIsReportedAheadOfWhatWasDerivedFromIt()
    {
        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings()));

        Assert.Equal(
            [
                "move Shared.received.rdoc",
                "move Shared.received.txt from Shared.received.rdoc",
                "move Shared#page_0001.received.rpage from Shared.received.rdoc",
                "move Shared#page_0001.received.txt from Shared.received.rdoc"
            ],
            reported);
    }

    /// <summary>
    /// What is reported in another order is still handed to the callbacks, and listed in the
    /// exception, in the order the targets came in.
    /// </summary>
    [Fact]
    public async Task CallbacksAndTheExceptionKeepTheOrderOfTheTargets()
    {
        var called = new List<string>();
        var settings = Settings();
        settings.OnFirstVerify(
            (file, _, _) =>
            {
                called.Add(Path.GetFileName(file.ReceivedPath));
                // Nothing has been reported while the callbacks run
                Assert.Empty(reported);
                return Task.CompletedTask;
            });

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", settings));

        string[] expected =
        [
            "Shared.received.txt",
            "Shared.received.rdoc",
            "Shared#page_0001.received.rpage",
            "Shared#page_0001.received.txt"
        ];
        Assert.Equal(expected, called);
        var positions = expected
            .Select(_ => exception.Message.IndexOf($"Received: {_}", StringComparison.Ordinal))
            .ToList();
        Assert.DoesNotContain(-1, positions);
        Assert.Equal(positions.OrderBy(_ => _), positions);
    }

    /// <summary>
    /// A document that has not changed has no received file to name, and no row in a tool for a
    /// file to be shown beneath.
    /// </summary>
    [Fact]
    public async Task ASourceThatPassedIsNotNamed()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared.verified.rdoc"), "doc");

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings()));

        Assert.Equal(
            [
                "move Shared.received.txt",
                "move Shared#page_0001.received.rpage",
                "move Shared#page_0001.received.txt"
            ],
            reported);
    }

    [Fact]
    public async Task ASourceThatWasAutoVerifiedIsNotNamed()
    {
        var settings = Settings();
        settings.AutoVerify(_ => _.EndsWith(".rdoc", StringComparison.Ordinal));

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", settings));

        Assert.Equal(
            [
                "move Shared.received.txt",
                "move Shared#page_0001.received.rpage",
                "move Shared#page_0001.received.txt"
            ],
            reported);
    }

    /// <summary>
    /// An info passed to the verification is the test's, so the file holding it is not something
    /// the document alone accounts for.
    /// </summary>
    [Fact]
    public async Task AnInfoTheTestAddedToIsNotDerived()
    {
        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings(), info: "from the test"));

        Assert.Equal(
            [
                "move Shared.received.txt",
                "move Shared.received.rdoc",
                "move Shared#page_0001.received.rpage from Shared.received.rdoc",
                "move Shared#page_0001.received.txt from Shared.received.rdoc"
            ],
            reported);
    }

    /// <summary>
    /// One level is all a diff tool is told. A page of a document that was rendered from another
    /// is a file of the outer one, as the inner document is.
    /// </summary>
    [Fact]
    public async Task ANestedSourceNamesTheOutermostOne()
    {
        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("outer"), "router", Settings()));

        Assert.Equal(
            [
                "move Shared.received.router",
                "move Shared.received.txt from Shared.received.router",
                "move Shared.received.rdoc from Shared.received.router",
                "move Shared#page_0001.received.rpage from Shared.received.router",
                "move Shared#page_0001.received.txt from Shared.received.router"
            ],
            reported);
    }

    /// <summary>
    /// With the outer document unchanged, the inner one is the outermost that is pending.
    /// </summary>
    [Fact]
    public async Task ANestedSourceIsNamedWhenTheOneAboveItPassed()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared.verified.router"), "outer");

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("outer"), "router", Settings()));

        Assert.Equal(
            [
                "move Shared.received.txt",
                "move Shared.received.rdoc",
                "move Shared#page_0001.received.rpage from Shared.received.rdoc",
                "move Shared#page_0001.received.txt from Shared.received.rdoc"
            ],
            reported);
    }

    /// <summary>
    /// A page the document no longer has. There is no target to say what it came from, so it goes
    /// by there being one document pending, and follows the launch for it.
    /// </summary>
    [Fact]
    public async Task AStaleFileIsDeletedWithTheOneDocument()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared#page_0002.verified.rpage"), "a page the document had");

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings()));

        Assert.Equal("move Shared.received.rdoc", reported[0]);
        Assert.Equal("delete Shared#page_0002.verified.rpage from Shared.received.rdoc", reported[^1]);
        Assert.Equal(5, reported.Count);
    }

    /// <summary>
    /// With the document unchanged the delete stands alone, and is raised ahead of the moves as
    /// a delete always was.
    /// </summary>
    [Fact]
    public async Task AStaleFileOfADocumentThatPassedStandsAlone()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared.verified.rdoc"), "doc");
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared#page_0002.verified.rpage"), "a page the document had");

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings()));

        Assert.Equal("delete Shared#page_0002.verified.rpage", reported[0]);
        Assert.Equal(4, reported.Count);
    }

    /// <summary>
    /// With more than one document pending, a stale file belongs to the one its name carries on
    /// from, and to none where it carries on from neither.
    /// </summary>
    [Fact]
    public async Task AStaleFileIsDeletedWithTheDocumentItIsNamedAfter()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared#Attachment2.page_0002.verified.rpage"), "a page the second had");
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared#Another.verified.txt"), "of neither");
        List<Target> attachments =
        [
            new("rdoc", Stream("first"), "Attachment1"),
            new("rdoc", Stream("second"), "Attachment2")
        ];

        await Assert.ThrowsAsync<VerifyException>(() => Verify(attachments, Settings()));

        Assert.Equal("delete Shared#Another.verified.txt", reported[0]);
        Assert.Equal("delete Shared#Attachment2.page_0002.verified.rpage from Shared#Attachment2.received.rdoc", reported[^1]);
        Assert.Equal(
            [
                "move Shared#Attachment1.received.rdoc",
                "move Shared#Attachment2.received.rdoc"
            ],
            reported.Where(_ => _.StartsWith("move", StringComparison.Ordinal) && !_.Contains(" from ")));
        Assert.Contains("move Shared#Attachment1.received.txt from Shared#Attachment1.received.rdoc", reported);
        Assert.Contains("move Shared#Attachment2.page_0001.received.rpage from Shared#Attachment2.received.rdoc", reported);
    }

    /// <summary>
    /// With diff off for the verification no tool is told of the document, so there is nothing
    /// for a delete to be shown beneath. The delete itself is raised as it always was.
    /// </summary>
    [Fact]
    public async Task WithDiffOffNothingIsLaunchedAndADeleteStandsAlone()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared#page_0002.verified.rpage"), "a page the document had");
        var settings = Settings();
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", settings));

        Assert.Equal(["delete Shared#page_0002.verified.rpage"], reported);
    }

    /// <summary>
    /// The map is for tooling that finds the files on disk, where the document is as much there
    /// as its pages are, whatever a diff tool was told.
    /// </summary>
    [Fact]
    public async Task TheReceivedMapNamesTheSource()
    {
        var settings = Settings();
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", settings));

        var document = Path.Combine(temp.Path, "Shared.received.rdoc");
        Assert.Equal(
            [
                document,
                Path.Combine(temp.Path, "Shared.verified.rdoc")
            ],
            Map(document));
        Assert.Equal(
            [
                Path.Combine(temp.Path, "Shared#page_0001.received.rpage"),
                Path.Combine(temp.Path, "Shared#page_0001.verified.rpage"),
                document
            ],
            Map(Path.Combine(temp.Path, "Shared#page_0001.received.rpage")));
        Assert.Equal(document, Map(Path.Combine(temp.Path, "Shared.received.txt"))[2]);
    }

    /// <summary>
    /// The global inline switch takes the first target, which for a document is its info. That
    /// is a file of the document, so it stays one, whether or not the document itself is a target.
    /// The same info from a converter that tells no source from what it derived is inlined, as it
    /// always was, which is what shows the other two were declined for being a document's.
    /// </summary>
    [Fact]
    public async Task TheInfoOfADocumentIsNotInlinedByTheGlobalSwitch()
    {
        VerifierSettings.RegisterStreamConverter(
            "rsheets",
            (_, _, _) => new("info of the sheets", null, [new("txt", "a,b", "Sheet1")]));
        VerifierSettings.RegisterStreamConverter(
            "rplain",
            (_, _, _) => new("info of the plain", [new("txt", "a,b", "Sheet1")]));
        VerifierSettings.Inline();

        var document = await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rdoc", Settings()));
        Assert.DoesNotContain("InlineNew:", document.Message);

        var sheets = await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rsheets", Settings()));
        Assert.DoesNotContain("InlineNew:", sheets.Message);

        var plain = await Assert.ThrowsAsync<VerifyException>(() => Verify(Stream("doc"), "rplain", Settings())
            .Snapshot("info of the plain"));
        Assert.Contains("InlineNew:", plain.Message);
    }

    static MemoryStream Stream(string content) =>
        new(Encoding.UTF8.GetBytes(content));

    static string Read(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    static string From(string? source)
    {
        if (source is null)
        {
            return "";
        }

        return $" from {Path.GetFileName(source)}";
    }

    VerifySettings Settings()
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.UseFileName("Shared");
        settings.DisableRequireUniquePrefix();
        return settings;
    }

    // Maps are named from a hash of the received path, so they are located by content
    static string[] Map(string receivedPath)
    {
        var directory = Path.Combine(
            AttributeReader.GetIntermediateDirectory(typeof(SourceDerivedReportTests).Assembly),
            "VerifyReceived");
        return Directory.EnumerateFiles(directory)
            .Select(File.ReadAllLines)
            .Single(_ => _.Length > 0 && _[0] == receivedPath);
    }
}
