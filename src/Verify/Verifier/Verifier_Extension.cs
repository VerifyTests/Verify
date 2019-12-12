partial class Verifier
{
    string textExtension = "txt";

    public void UseExtensionForText(string extension)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        textExtension = extension;
    }
}