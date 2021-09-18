namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task VerifyFile(string path)
        {
            Guard.FileExists(path, nameof(path));
            settings.extension ??= EmptyFiles.Extensions.GetExtension(path);
            return VerifyStream(FileHelpers.OpenRead(path));
        }

        public Task VerifyFile(FileInfo target)
        {
            return VerifyFile(target.FullName);
        }
    }
}