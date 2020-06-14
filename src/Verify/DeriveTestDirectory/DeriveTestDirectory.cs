using System.Runtime.CompilerServices;

namespace VerifyTests
{
    /// <summary>
    /// Used to use a custom directory to search for `.verified.` files.
    /// </summary>
    /// <param name="sourceFile">The source file derived from <see cref="CallerFilePathAttribute"/>.</param>
    /// <param name="projectDirectory">The directory of the project that the test was compile from.</param>
    /// <returns>A new value or `null` to use the default behavior.</returns>
    public delegate string? DeriveTestDirectory(string sourceFile, string projectDirectory);
}