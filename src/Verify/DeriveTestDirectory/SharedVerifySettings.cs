namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static DeriveTestDirectory deriveTestDirectory = _ => _;

        /// <summary>
        /// Used to use a custom directory to search for `.verified.` files.
        /// </summary>
        /// <param name="deriveTestDirectory">Custom callback to control the behavior.</param>
        public static void DeriveTestDirectory(DeriveTestDirectory deriveTestDirectory)
        {
            Guard.AgainstNull(deriveTestDirectory, nameof(deriveTestDirectory));
            SharedVerifySettings.deriveTestDirectory =
                directory =>
                {
                    var result = deriveTestDirectory(directory);
                    Guard.DirectoryExists(result, nameof(result));
                    return result;
                };
        }
    }
}