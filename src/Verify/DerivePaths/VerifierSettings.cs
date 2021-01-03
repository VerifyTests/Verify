using System;
using System.IO;
using System.Reflection;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static DerivePathInfo? derivePathInfo;

        internal static PathInfo GetPathInfo(string sourceFile, string projectDirectory, Type type, MethodInfo method)
        {
            if (derivePathInfo != null)
            {
                var pathInfo = derivePathInfo(sourceFile, projectDirectory, type, method);
                Directory.CreateDirectory(pathInfo.Directory);
                return pathInfo;
            }

            string name;
            if (type.IsNested)
            {
                name = $"{type.ReflectedType!.Name}.{type.Name}.{method.Name}";
            }
            else
            {
                name = $"{type.Name}.{method.Name}";
            }

            return new PathInfo(Path.GetDirectoryName(sourceFile)!, name);

        }

        /// <summary>
        /// Use custom path information for `.verified.` files.
        /// </summary>
        /// <remarks>
        /// This is sometimes needed on CI systems that move/remove the original source.
        /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
        /// </remarks>
        /// <param name="derivePathInfo">Custom callback to control the behavior.</param>
        public static void DerivePathInfo(DerivePathInfo derivePathInfo)
        {
            Guard.AgainstNull(derivePathInfo, nameof(derivePathInfo));
            VerifierSettings.derivePathInfo = derivePathInfo;
        }
    }
}