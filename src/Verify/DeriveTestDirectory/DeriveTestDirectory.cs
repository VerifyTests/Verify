using System.Runtime.CompilerServices;

namespace Verify
{
    /// <summary>
    /// Used to use a custom directory to search for `.verified.` files.
    /// </summary>
    /// <param name="testDirectory">The directory derived from <see cref="CallerFilePathAttribute"/>.</param>
    /// <param name="projectDirectory">The directory of the project that the test was compile from.</param>
    /// <returns>A new value or <paramref name="testDirectory"/> to use the default behavior.</returns>
    public delegate string DeriveTestDirectory(string testDirectory, string projectDirectory);
}