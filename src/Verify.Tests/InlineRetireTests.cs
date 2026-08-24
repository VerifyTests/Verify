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
        // Nothing here should open a window; a blocked launch still reports its move to the owner,
        // which the assertions below filter out.
        DiffRunner.MaxInstancesToLaunch(0);
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

        // The verification itself is beside the point: the call site stops being an inline
        // snapshot whether the file snapshot then passes or fails, so the retire goes either way.
        await Record.ExceptionAsync(() => Verify("value", settings));

        var settle = listener.AwaitSettle();
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
        File.WriteAllText(receivedFile, "staged content");
        File.WriteAllText(expectedFile, "old");

        var settings = new VerifySettings();
        settings.UseDirectory(listener.Directory);
        settings.NotInline();

        await Record.ExceptionAsync(() => Verify("value", settings));

        Assert.False(File.Exists(patchFile));
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(expectedFile));
    }

    static string SourceFile([CallerFilePath] string file = "") =>
        InnerVerifier.MapSourceFile(file);

    sealed record Settle(string? Key, string? Origin, string? Member);

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
        /// The first settle to arrive, or null. Other verbs reach the owner on the same port — a
        /// blocked diff launch still reports its move — so this reads past them.
        /// </summary>
        public Settle? AwaitSettle()
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (DateTime.UtcNow < deadline)
            {
                while (payloads.TryDequeue(out var payload))
                {
                    if (TryReadSettle(payload, out var settle))
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
                }
            }

            if (verb != "settle")
            {
                return false;
            }

            settle = new(key, origin, member);
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
