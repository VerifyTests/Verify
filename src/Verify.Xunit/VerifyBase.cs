namespace VerifyXunit;

[UsesVerify]
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

    public SettingsTask Verify<T>(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, rawTargets , settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        IEnumerable<Target> targets,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(targets , settings, sourceFile);
    }
}