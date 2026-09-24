using System.Net;
using System.Net.Sockets;

/// <summary>
/// A verification that is not inline has to drop whatever a previous run queued for its call site.
/// Nothing else will: settling only ever happened on the inline path, so a test switched to
/// <c>NotInline</c> left its snapshot pending for good.
/// <para>
/// Driven against a real socket rather than a stand in, because the whole point is that the
/// message leaves the process. What answers it here is a listener that records payloads, so the
/// assertions are on the wire itself.
/// </para>
/// </summary>
[Collection("Inline")]
public class InlineRetireTests :
    IDisposable
{
    // ViewerClient reads this on every call, and DiffEngine keeps it internal, so it is named here.
    const string portVariable = "DiffEngine_ViewerPort";

    readonly Func<bool> originalIsBuildServer = InlineEngine.IsBuildServer;
    readonly bool originalDisabled = DiffRunner.Disabled;
    readonly string? originalPort = Environment.GetEnvironmentVariable(portVariable);
    readonly Listener listener = new();

    public InlineRetireTests()
    {
        InlineEngine.IsBuildServer = () => false;
        // DiffEngine switches diff off by itself on a build server, under continuous testing and
        // under an AI CLI, and a retire rides the same switch.
        DiffRunner.Disabled = false;
        Environment.SetEnvironmentVariable(portVariable, listener.Port.ToString());
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(portVariable, originalPort);
        DiffRunner.Disabled = originalDisabled;
        InlineEngine.IsBuildServer = originalIsBuildServer;
        listener.Dispose();
    }

    [Fact]
    public async Task NotInlineRetiresTheCallSite()
    {
        var settings = new VerifySettings();
        settings.UseDirectory(listener.Directory);
        settings.NotInline();

        // The file verification has to pass. A failing one hands a pending move to whatever owns
        // the queue on this machine, and on a developer box that is the tray - which shows the
        // move and then drops it again when the temp directory goes. Turning diff off is not open
        // to this test: Retire rides settings.diffEnabled too, so it would retire nothing.
        // RetireInline runs before the file pipeline, so accepting the snapshot costs it nothing.
        settings.AutoVerify();

        // The verification itself is beside the point: the call site stops being an inline
        // snapshot whether the file snapshot then passes or fails, so the retire goes either way.
        await Verify("value", settings);

        var settle = listener.AwaitSettle(nameof(NotInlineRetiresTheCallSite));
        Assert.NotNull(settle);
        Assert.NotNull(settle.Key);

        // The key names this file and the line of the verify call above. Built through InlineKey
        // so the file is folded exactly as the sender folds it, then with the line taken back off.
        var expectedFile = InlineKey.For(SourceFile(), 0)[..^1];
        Assert.StartsWith(expectedFile, settle.Key!);

        // The member is what finds the entry once an accept elsewhere in the file has moved that
        // line past it.
        Assert.Equal(nameof(NotInlineRetiresTheCallSite), settle.Member);

        // No framework: the statement is "there is no inline snapshot here", not "this framework
        // now passes", so the owner takes the whole entry rather than one variant of it.
        Assert.Null(settle.Origin);

        // And no value, since no call site here holds an inline snapshot to settle by
        Assert.Null(settle.Value);
    }

    /// <summary>
    /// A passing inline verification settles with the value its expected argument holds. Where the
    /// line names no entry the owner falls back to the member, and a member is not a call site: the
    /// value is what keeps a passing call from settling the entry of a failing sibling beside it.
    /// </summary>
    [Fact]
    public async Task APassingSnapshotSettlesWithItsValue()
    {
        // Nothing is at this path. A settle names the call site and reads nothing from it
        var source = Path.Combine(listener.Directory, "Snapshot.cs");

        var settings = new VerifySettings();
        settings.UseDirectory(listener.Directory);
        settings.Snapshot("value", source, 7, "\"value\"");

        await Verify("value", settings);

        var settle = listener.AwaitSettle(nameof(APassingSnapshotSettlesWithItsValue));
        Assert.NotNull(settle);
        Assert.Equal(InlineKey.For(InnerVerifier.MapSourceFile(source), 7), settle.Key);
        Assert.Equal("value", settle.Value);
    }

    /// <summary>
    /// The same on disk. A trio staged for another call site in this member - a failing sibling -
    /// is found by the member fallback, and only the value says it is not this call's: the passing
    /// call holds neither its anchor nor its content. Without the value it was deleted.
    /// </summary>
    [Fact]
    public async Task APassingSnapshotLeavesASiblingsStagedSnapshot()
    {
        var intermediate = VerifierSettings.IntermediateDir;
        Assert.NotNull(intermediate);

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        var staging = Path.Combine(intermediate!, InlineStaging.DirectoryName);
        Directory.CreateDirectory(staging);

        var source = Path.Combine(listener.Directory, "Sibling.cs");
        var stem = nameof(APassingSnapshotLeavesASiblingsStagedSnapshot);
        var patchFile = Path.Combine(staging, $"{stem}.inlinepatch");
        var receivedFile = Path.Combine(staging, $"{stem}.received.txt");
        var expectedFile = Path.Combine(staging, $"{stem}.expected.txt");
        InlinePatchFile.Write(
            patchFile,
            new(InnerVerifier.MapSourceFile(source), 20, "\"old\"", "staged content")
            {
                TestName = $"InlineRetireTests.{stem}",
                MemberName = stem,
                OriginalValue = "old"
            });
        await File.WriteAllTextAsync(receivedFile, "staged content");
        await File.WriteAllTextAsync(expectedFile, "old");

        try
        {
            var settings = new VerifySettings();
            settings.UseDirectory(listener.Directory);
            settings.Snapshot("value", source, 7, "\"value\"");

            await Verify("value", settings);

            Assert.True(File.Exists(patchFile));
            Assert.True(File.Exists(receivedFile));
            Assert.True(File.Exists(expectedFile));
        }
        finally
        {
            File.Delete(patchFile);
            File.Delete(receivedFile);
            File.Delete(expectedFile);
        }
    }

    /// <summary>
    /// A declined Snapshot call is a call site of its own, and it is the one whose entry is pending:
    /// a patch for a literal already in the source is keyed by the Snapshot call's line, not the
    /// verify call's. The same goes for a literal that outgrew the size limit.
    /// </summary>
    [Fact]
    public async Task ADeclinedSnapshotRetiresItsOwnCallSite()
    {
        // Nothing is at this path. Declining a literal strips it from the file it names, and this is
        // about the retire alone
        var source = Path.Combine(listener.Directory, "Snapshot.cs");

        var settings = new VerifySettings();
        settings.UseDirectory(listener.Directory);
        settings.Snapshot("value", source, 7, "\"value\"");
        settings.NotInline();

        // Accepted rather than left failing, for the reason given in NotInlineRetiresTheCallSite
        settings.AutoVerify();

        await Verify("value", settings);

        var settle = listener.AwaitSettle(nameof(ADeclinedSnapshotRetiresItsOwnCallSite));
        Assert.NotNull(settle);
        Assert.Equal(InlineKey.For(InnerVerifier.MapSourceFile(source), 7), settle.Key);
    }

    /// <summary>
    /// A settle only ever reached the queue owner, which says nothing to a snapshot that is on
    /// disk instead — staged by a run that found no owner, or written out by one on its way out.
    /// Those files are what accept tooling reads, so the snapshot stayed pending for a test that
    /// had stopped being inline.
    /// </summary>
    [Fact]
    public async Task NotInlineClearsStagedFilesForTheCallSite()
    {
        var intermediate = VerifierSettings.IntermediateDir;
        Assert.NotNull(intermediate);

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        var staging = Path.Combine(intermediate!, InlineStaging.DirectoryName);
        Directory.CreateDirectory(staging);

        // A deliberately wrong line, so this only clears by way of the member fallback — which is
        // the case that matters, since a staged snapshot outlives the edits that move its line.
        var stem = nameof(NotInlineClearsStagedFilesForTheCallSite);
        var patchFile = Path.Combine(staging, $"{stem}.inlinepatch");
        var receivedFile = Path.Combine(staging, $"{stem}.received.txt");
        var expectedFile = Path.Combine(staging, $"{stem}.expected.txt");
        InlinePatchFile.Write(
            patchFile,
            new(SourceFile(), 1, "\"old\"", "staged content")
            {
                TestName = $"InlineRetireTests.{stem}",
                MemberName = stem,
                OriginalValue = "old"
            });
        await File.WriteAllTextAsync(receivedFile, "staged content");
        await File.WriteAllTextAsync(expectedFile, "old");

        var settings = new VerifySettings();
        settings.UseDirectory(listener.Directory);
        settings.NotInline();

        // Accepted rather than left failing, for the reason given in NotInlineRetiresTheCallSite
        settings.AutoVerify();

        await Verify("value", settings);

        Assert.False(File.Exists(patchFile));
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(expectedFile));
    }

    static string SourceFile([CallerFilePath] string file = "") =>
        InnerVerifier.MapSourceFile(file);

    sealed record Settle(string? Key, string? Origin, string? Member, string? Value);

    /// <summary>
    /// Stands in for whoever owns the inline queue: accepts connections, records what arrives, and
    /// answers so the sender is not left waiting out its timeout.
    /// </summary>
    sealed class Listener :
        IDisposable
    {
        readonly TcpListener tcp;
        readonly CancelSource cancellation = new();
        readonly ConcurrentQueue<string> payloads = new();
        readonly TempDirectory directory = new();

        public Listener()
        {
            tcp = new(IPAddress.Loopback, 0);
            tcp.Start();
            Port = ((IPEndPoint) tcp.LocalEndpoint).Port;
            _ = Task.Run(Accept);
        }

        public int Port { get; }

        public string Directory => directory.Path;

        async Task Accept()
        {
            while (!cancellation.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    // No token overload on net48, so shutdown arrives as the stopped listener
                    // faulting this await.
                    client = await tcp.AcceptTcpClientAsync();
                }
                catch
                {
                    return;
                }

                try
                {
                    using (client)
                    {
                        // ReSharper disable once UseAwaitUsing
                        using var stream = client.GetStream();
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        payloads.Enqueue(await reader.ReadToEndAsync());
                        var response = "version: 1\nstatus: ok\n"u8.ToArray();
                        await stream.WriteAsync(response);
                        await stream.FlushAsync();
                    }
                }
                catch
                {
                    // A sender that vanished mid exchange. Nothing to record.
                }
            }
        }

        /// <summary>
        /// The first settle for <paramref name="member"/> to arrive, or null. Other verbs, and
        /// settles for other call sites, can reach the owner on the same port, so this reads past them.
        /// </summary>
        public Settle? AwaitSettle(string member)
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (DateTime.UtcNow < deadline)
            {
                while (payloads.TryDequeue(out var payload))
                {
                    if (TryReadSettle(payload, out var settle) &&
                        settle!.Member == member)
                    {
                        return settle;
                    }
                }

                Thread.Sleep(20);
            }

            return null;
        }

        static bool TryReadSettle(string payload, out Settle? settle)
        {
            settle = null;
            string? verb = null;
            string? key = null;
            string? origin = null;
            string? member = null;
            string? settledBy = null;
            foreach (var raw in payload.Replace("\r\n", "\n").Split('\n'))
            {
                var separator = raw.IndexOf(':');
                if (separator < 1)
                {
                    continue;
                }

                var name = raw[..separator];
                var value = raw[(separator + 1)..].Trim();
                switch (name)
                {
                    case "verb":
                        verb = value;
                        break;
                    case "key":
                        key = Decode(value);
                        break;
                    case "body":
                        origin = Decode(value);
                        break;
                    case "member":
                        member = Decode(value);
                        break;
                    case "value":
                        settledBy = Decode(value);
                        break;
                }
            }

            if (verb != "settle")
            {
                return false;
            }

            settle = new(key, origin, member, settledBy);
            return true;
        }

        static string? Decode(string value)
        {
            if (value.Length == 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public void Dispose()
        {
            cancellation.Cancel();
            tcp.Stop();
            cancellation.Dispose();
            directory.Dispose();
        }
    }
}
