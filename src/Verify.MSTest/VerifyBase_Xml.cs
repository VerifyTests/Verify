using System.Xml;
using System.Xml.Linq;

namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask VerifyXml(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask VerifyXml(
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask VerifyXml(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask Verify(
        XDocument? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask Verify(
        Task<XDocument> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask Verify(
        XmlDocument? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public SettingsTask Verify(
        Task<XmlDocument> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));
}