namespace VerifyTests;

[Experimental("RegisterStreamScrubber")]
public delegate Task<StreamScrubberResult?> StreamScrubber(string? name, Stream target, IReadOnlyDictionary<string, object> context);

[Experimental("RegisterStreamScrubber")]
public readonly struct StreamScrubberResult(Stream stream, Func<Task>? cleanup = null)
{
    public Stream Stream { get; } = stream;
    public Func<Task>? Cleanup { get; } = cleanup;
}