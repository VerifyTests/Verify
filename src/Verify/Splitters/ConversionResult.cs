namespace VerifyTests;

public readonly struct ConversionResult
{
    public object? Info { get; }

    public IEnumerable<Target> Targets { get; }

    public Func<Task>? Cleanup { get; }

    public ConversionResult(object? info, IEnumerable<Target> targets, Func<Task>? cleanup = null)
    {
        Info = info;
        Targets = targets;
        Cleanup = cleanup;
    }

    public ConversionResult(object? info, string extension, Stream stream, Func<Task>? cleanup = null)
    {
        Guard.AgainstNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, stream)];
    }

    public ConversionResult(object? info, string extension, string data, Func<Task>? cleanup = null)
    {
        Guard.AgainstNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, data)];
    }

    public ConversionResult(object? info, string extension, StringBuilder data, Func<Task>? cleanup = null)
    {
        Guard.AgainstNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, data)];
    }
}