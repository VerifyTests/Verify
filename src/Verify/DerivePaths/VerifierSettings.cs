using System.IO;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static DeriveDirectory? deriveDirectory;

        internal static string DeriveDirectory(string sourceFile, string projectDirectory)
        {
            if (deriveDirectory == null)
            {
                return Path.GetDirectoryName(sourceFile)!;
            }

            var directory = deriveDirectory(sourceFile, projectDirectory);
            if (directory == null)
            {
                return Path.GetDirectoryName(sourceFile)!;
            }

            return directory;
        }

        /// <summary>
        /// Use a custom directory for `.verified.` files.
        /// </summary>
        /// <remarks>
        /// This is sometimes needed on CI systems that move/remove the original source.
        /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
        /// </remarks>
        /// <param name="deriveDirectory">Custom callback to control the behavior.</param>
        public static void DeriveDirectory(DeriveDirectory deriveDirectory)
        {
            Guard.AgainstNull(deriveDirectory, nameof(deriveDirectory));
            VerifierSettings.deriveDirectory =
                (sourceFile, projectDirectory) =>
                {
                    var result = deriveDirectory(sourceFile, projectDirectory);
                    if (result != null)
                    {
                        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
                        result = Path.Combine(sourceFileDirectory, result);
                        Directory.CreateDirectory(result);
                    }

                    return result;
                };
        }
    }
}