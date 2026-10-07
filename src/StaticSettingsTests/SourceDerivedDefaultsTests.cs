using System.Net;
using System.Net.Sockets;
using DiffEngine;

// What Verify says to DiffEngine of a source, on the wire. SourceDerivedReportTests stands in for
// the two calls that say it, so that nothing reaches a tray or a viewer on the machine, and so
// never runs them. Here they run, against a listener that is the queue owner, which is as far as
// a message can be followed from this side: what an owner does with one is DiffEngine's to test.
public class SourceDerivedDefaultsTests :
    BaseTest,
    IDisposable
{
    // ViewerClient reads this on every call, and DiffEngine keeps it internal, so it is named here.
    const string portVariable = "DiffEngine_ViewerPort";

    bool originalDisabled = DiffRunner.Disabled;
    bool originalTrayDisabled = DiffRunner.TrayDisabled;
    string? originalPort = Environment.GetEnvironmentVariable(portVariable);
    Listener listener = new();
    TempDirectory temp = new();

    public SourceDerivedDefaultsTests()
    {
        // A tray running on this machine would otherwise be told ahead of the listener
        DiffRunner.TrayDisabled = true;
        Environment.SetEnvironmentVariable(portVariable, listener.Port.ToString());
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(portVariable, originalPort);
        DiffRunner.TrayDisabled = originalTrayDisabled;
        DiffRunner.Disabled = originalDisabled;
        listener.Dispose();
        temp.Dispose();
    }

    /// <summary>
    /// A file derived from a pending source names it, and one that stands alone names nothing,
    /// whether the file is text or not: those are four calls, and each has to say the same.
    /// </summary>
    [Theory]
    [InlineData("png", true)]
    [InlineData("txt", true)]
    [InlineData("png", false)]
    [InlineData("txt", false)]
    public async Task AMoveIsToldWithItsSource(string extension, bool derived)
    {
        // The call under test, and not a stand in another test left behind
        Assert.Equal("DefaultLaunchDiff", VerifyEngine.LaunchDiff.Method.Name);

        // Launching off, so no tool is resolved or opened for the pair. It is tracked all the same
        DiffRunner.Disabled = true;
        var received = Path.Combine(temp, $"Shared#page_0001.received.{extension}");
        var verified = Path.Combine(temp, $"Shared#page_0001.verified.{extension}");
        await File.WriteAllTextAsync(received, "received");
        var source = Source(derived);

        await VerifyEngine.LaunchDiff(new(extension, received, verified), source);

        Assert.Equal(new("move", received, verified, source), Assert.Single(listener.Heard()));
    }

    /// <summary>
    /// The same of a verified file no target produced: a page the document no longer has names
    /// the document, and a file that stands alone names nothing.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ADeleteIsToldWithItsSource(bool derived)
    {
        Assert.Equal("DefaultAddDelete", RaisedDeletes.AddDelete.Method.Name);

        // A delete is not raised at all while launching is off, and one the listener holds
        // starts nothing
        DiffRunner.Disabled = false;
        var stale = Path.Combine(temp, "Shared#page_0002.verified.png");
        var source = Source(derived);

        await RaisedDeletes.AddDelete(stale, source);

        Assert.Equal(new("delete", stale, null, source), Assert.Single(listener.Heard()));
    }

    // The received file of the document, which is all a source is to DiffEngine: a path it is
    // told, of a file it never opens
    string? Source(bool derived)
    {
        if (derived)
        {
            return Path.Combine(temp, "Shared.received.rdoc");
        }

        return null;
    }

    // ReSharper disable NotAccessedPositionalProperty.Local
    record Heard(string? Verb, string? Key, string? Body, string? Source);
    // ReSharper restore NotAccessedPositionalProperty.Local

    /// <summary>
    /// A queue owner that only writes down what it was sent, and answers that it took it.
    /// </summary>
    sealed class Listener :
        IDisposable
    {
        readonly TcpListener tcp;
        readonly ConcurrentQueue<string> payloads = new();

        public Listener()
        {
            tcp = new(IPAddress.Loopback, 0);
            tcp.Start();
            Port = ((IPEndPoint) tcp.LocalEndpoint).Port;
            _ = Task.Run(Accept);
        }

        public int Port { get; }

        async Task Accept()
        {
            while (true)
            {
                TcpClient client;
                try
                {
                    // Shutdown arrives as the stopped listener faulting this await
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
                        // Written down before it is answered, and the sender waits for the answer,
                        // so what a call sent is here by the time the call returns
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

        public List<Heard> Heard()
        {
            var heard = new List<Heard>();
            foreach (var payload in payloads)
            {
                heard.Add(Read(payload));
            }

            return heard;
        }

        static Heard Read(string payload)
        {
            string? verb = null;
            string? key = null;
            string? body = null;
            string? source = null;
            foreach (var raw in payload.Replace("\r\n", "\n").Split('\n'))
            {
                var separator = raw.IndexOf(':');
                if (separator < 1)
                {
                    continue;
                }

                var value = raw[(separator + 1)..].Trim();
                switch (raw[..separator])
                {
                    case "verb":
                        verb = value;
                        break;
                    case "key":
                        key = Decode(value);
                        break;
                    case "body":
                        body = Decode(value);
                        break;
                    case "source":
                        source = Decode(value);
                        break;
                }
            }

            return new(verb, key, body, source);
        }

        // Everything but the verb crosses as base64, so a path can hold anything a line cannot
        static string? Decode(string value)
        {
            if (value.Length == 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public void Dispose() =>
            tcp.Stop();
    }
}
