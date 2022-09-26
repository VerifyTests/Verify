namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask Verify(
        FileStream? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<FileStream> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        Stream? target,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null)
        where T : Stream
    {
        settings ??= this.settings;
        return Verifier.Verify(targets, extension, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        Task<T> target,
        string extension,
        VerifySettings? settings = null)
        where T : Stream
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }

    public SettingsTask Verify(
        byte[]? target,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }
}