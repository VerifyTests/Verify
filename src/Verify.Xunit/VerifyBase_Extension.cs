namespace VerifyXunit
{
    public partial class VerifyBase
    {
        string extension = ".txt";

        public void UseExtension(string extension)
        {
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            this.extension = extension;
        }
    }
}