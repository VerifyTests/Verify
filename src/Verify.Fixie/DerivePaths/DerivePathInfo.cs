namespace VerifyFixie;

/// <summary>
/// Signature for deriving a custom path information for `.verified.` files.
/// </summary>
/// <param name="sourceFile">The source file derived from <see cref="CallerFilePathAttribute" />.</param>
/// <param name="projectDirectory">The directory of the project that the test was compile from.</param>
/// <param name="type">The class the test method exists in.</param>
/// <param name="method">The test method.</param>
public delegate PathInfo DerivePathInfo(string sourceFile, string projectDirectory, Type type, MethodInfo method);