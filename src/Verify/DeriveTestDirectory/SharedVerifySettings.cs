using System;
using System.Linq;
using System.Reflection;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static DeriveTestDirectory? deriveDirectory;

        internal static string DeriveDirectory(Type type, string testDirectory)
        {
            if (deriveDirectory == null)
            {
                return testDirectory;
            }

            var projectDirectory = type.Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory")?.Value;
            return deriveDirectory(type, testDirectory, projectDirectory);
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
                (type, testDirectory, projectDirectory) =>
                {
                    var result = deriveTestDirectory(type, testDirectory, projectDirectory);
                    Guard.DirectoryExists(result, nameof(result));
                    return result;
                };
        }
    }
}