using System.IO;
using System.Linq;
using System.Reflection;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static DeriveTestDirectory? deriveDirectory;

        internal static string DeriveDirectory(string sourceFile, Assembly assembly)
        {
            if (deriveDirectory == null)
            {
                return Path.GetDirectoryName(sourceFile);
            }

            var projectDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory")
                ?.Value;
            if (projectDirectory == null)
            {
                throw InnerVerifier.exceptionBuilder("Using `DeriveTestDirectory` requires that the test assembly be initialized at assembly load time. Call `SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());`.");
            }
            var directory = deriveDirectory(sourceFile, projectDirectory);
            if (directory == null)
            {
                return Path.GetDirectoryName(sourceFile);
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