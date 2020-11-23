using System.IO;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static DeriveTestDirectory? deriveDirectory;

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
        /// Use a custom directory to find `.verified.` files.
        /// </summary>
        /// <remarks>
        /// This is sometimes needed on CI systems that move/remove the original source.
        /// To use this approach, `.verified.` files will need to be replicated to the new directory
        /// </remarks>
        /// <param name="deriveTestDirectory">Custom callback to control the behavior.</param>
        public static void DeriveTestDirectory(DeriveTestDirectory deriveTestDirectory)
        {
            Guard.AgainstNull(deriveTestDirectory, nameof(deriveTestDirectory));
            deriveDirectory =
                (sourceFile, projectDirectory) =>
                {
                    var result = deriveTestDirectory(sourceFile, projectDirectory);
                    if (result != null)
                    {
                        Guard.DirectoryExists(result, nameof(result));
                    }

                    return result;
                };
        }
    }
}