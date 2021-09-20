using System.Linq;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public partial class InnerVerifier :
        IDisposable
    {
        VerifySettings settings;
        internal GetFileNames GetFileNames{ get; }
        internal GetIndexedFileNames GetIndexedFileNames{ get; }
        internal List<string> VerifiedFiles { get; }
        internal List<string> ReceivedFiles { get; }

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method)
        {
            var (fileNamePrefix, directory) = FileNameBuilder.FileNamePrefix(method, type, sourceFile, settings);

            var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
            if (directory is null)
            {
                directory = sourceFileDirectory;
            }
            else
            {
                directory = Path.Combine(sourceFileDirectory, directory);
                Directory.CreateDirectory(directory);
            }

            var filePathPrefix = Path.Combine(directory, fileNamePrefix);
            PrefixUnique.CheckPrefixIsUnique(filePathPrefix);

            var pattern = $"{fileNamePrefix}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".verified").ToList();
            ReceivedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".received").ToList();

            GetFileNames = extension => new(extension, filePathPrefix);
            GetIndexedFileNames = (extension, index) => new(extension, $"{filePathPrefix}.{index:D2}");

            foreach (var file in ReceivedFiles)
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