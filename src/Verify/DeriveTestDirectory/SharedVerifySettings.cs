using System;
using System.Linq;
using System.Reflection;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static DeriveTestDirectory? deriveTestDirectory;

        internal static string DeriveDirectory(Type testType, string testDirectory)
        {
            if (deriveTestDirectory == null)
            {
                return testDirectory;
            }

            var projectAttribute = testType.Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory");
            if (projectAttribute == null)
            {
                return testDirectory;
            }
            return deriveTestDirectory(testDirectory, projectAttribute.Value);
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
            SharedVerifySettings.deriveTestDirectory =
                (testDirectory, projectDirectory) =>
                {
                    var result = deriveTestDirectory(testDirectory, projectDirectory);
                    Guard.DirectoryExists(result, nameof(result));
                    return result;
                };
        }
    }
}