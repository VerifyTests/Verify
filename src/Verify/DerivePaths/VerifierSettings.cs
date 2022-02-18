// ReSharper disable UnusedParameter.Local

namespace VerifyTests;

public static partial class VerifierSettings
{
    #region defaultDerivePathInfo
    static DerivePathInfo derivePathInfo = (sourceFile, projectDirectory, type, method) =>
    {
        static string GetTypeName(Type type)
        {
            if (type.IsNested)
            {
                return $"{type.ReflectedType!.Name}.{type.Name}";
            }

            return type.Name;
        }

        var typeName = GetTypeName(type);

        return new(Path.GetDirectoryName(sourceFile)!, typeName, method.Name);
    };
    #endregion

    internal static PathInfo GetPathInfo(string sourceFile, Type type, MethodInfo method)
    {
        return derivePathInfo(sourceFile, TargetAssembly.ProjectDir, type, method);
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
        VerifierSettings.derivePathInfo = derivePathInfo;
    }
}