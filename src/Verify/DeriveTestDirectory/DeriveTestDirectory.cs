using System.Runtime.CompilerServices;

namespace Verify
{
    /// <summary>
    /// Used to use a custom directory to search for `.verified.` files.
    /// </summary>
    /// <param name="directory">The directory derived from <see cref="CallerFilePathAttribute"/>.</param>
    /// <returns>A new value or <paramref name="directory"/> to use the default behavior.</returns>
    public delegate string DeriveTestDirectory(string directory);
}