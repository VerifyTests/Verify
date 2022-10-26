using System.Xml;
using System.Xml.Linq;

namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask VerifyXml(
        string? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyXml(target, settings, sourceFile);
    }

    public SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyXml(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        XDocument? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        XmlDocument? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }
}