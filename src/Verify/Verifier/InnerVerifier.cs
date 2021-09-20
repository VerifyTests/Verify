using System.Linq;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public  delegate (string fileNamePrefix, string? directory) GetFileConvention(string uniquenessParts);
    
    public partial class InnerVerifier :
        IDisposable
    {
        VerifySettings settings;
        internal GetFileNames GetFileNames { get; }
        internal GetIndexedFileNames GetIndexedFileNames { get; }
        internal List<string> VerifiedFiles { get; }
        internal List<string> ReceivedFiles { get; }

        public InnerVerifier(string sourceFile, VerifySettings settings, GetFileConvention fileConvention)
        {
            this.settings = settings;

            var uniquenessParts = PrefixUnique.GetUniquenessParts(settings.Namer);
            (string fileNamePrefix, var directory) = fileConvention( uniquenessParts);

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


            VerifierSettings.RunBeforeCallbacks();
        }

        public void Dispose()
        {
            VerifierSettings.RunAfterCallbacks();
        }
    }
}