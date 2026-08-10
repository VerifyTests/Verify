namespace VerifyNUnit;

[TestFixture]
public abstract partial class VerifyBase
{
    VerifySettings? settings;
    string sourceFile;

    public VerifyBase(VerifySettings? settings = null, [CallerFilePath] string sourceFile = "")
    {
        if (string.IsNullOrWhiteSpace(sourceFile))
        {
            throw new($"{nameof(VerifyBase)}.ctor must be called explicitly.");
        }

        this.settings = settings;
        this.sourceFile = sourceFile;
    }

    [Pure]
    public SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, rawTargets, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(targets, settings ?? this.settings, sourceFile, lineNumber);
}