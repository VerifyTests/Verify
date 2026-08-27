namespace VerifyTests;

/// <summary>
/// Decides whether a verification uses an inline snapshot.
/// </summary>
/// <param name="typeName">The test class.</param>
/// <param name="methodName">The test method.</param>
/// <param name="sourceFile">The source file the verify call is in.</param>
/// <param name="extension">The extension of the first target, which is the one that would be inlined.</param>
public delegate bool GlobalInline(string typeName, string methodName, string sourceFile, string extension);
