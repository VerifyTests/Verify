namespace VerifyXunit
{
    public partial class VerifyBase
    {
        string textExtension = ".txt";

        public void UseExtensionForText(string extension)
        {
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            textExtension = extension;
        }
    }
}