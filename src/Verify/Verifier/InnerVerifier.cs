namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public partial class InnerVerifier :
        IDisposable
    {
        VerifySettings settings;
        FileNameBuilder fileNameBuilder;

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method)
        {
            fileNameBuilder = new(method, type, sourceFile, settings);
            
            foreach (var file in fileNameBuilder.ReceivedFiles)
            {
                File.Delete(file);
            }

            this.settings = settings;

            VerifierSettings.RunBeforeCallbacks();
        }

        public void Dispose()
        {
            VerifierSettings.RunAfterCallbacks();
        }
    }
}