namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask Verify(
        FileStream stream,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(stream, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<FileStream> stream,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(stream, settings, sourceFile);
    }

    public SettingsTask Verify(
        Stream stream,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(stream, extension, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        Task<T> stream,
        string extension,
        VerifySettings? settings = null)
        where T : Stream
    {
        settings ??= this.settings;
        return Verifier.Verify(stream, extension, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        IEnumerable<T?> streams,
        string extension,
        VerifySettings? settings = null)
        where T : Stream
    {
        settings ??= this.settings;
        return Verifier.Verify(streams, extension, settings, sourceFile);
    }

    public SettingsTask Verify(
        byte[] bytes,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(bytes, extension, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<byte[]> bytes,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(bytes, extension, settings, sourceFile);
    }
}