namespace VerifyTests;

/// <param name="verifiedFile">
/// The <c>.verified</c> file about to be written, or - for an
/// <see href="https://github.com/VerifyTests/Verify/blob/main/docs/inline-snapshots.md">inline
/// snapshot</see> - the test source file the literal lives in, since that is what accepting
/// rewrites. A delegate that decides by matching the verified file convention therefore declines
/// every inline snapshot, and one that decides by directory accepts them.
/// </param>
public delegate bool AutoVerify(string verifiedFile);

/// <inheritdoc cref="AutoVerify" />
public delegate bool GlobalAutoVerify(string typeName, string methodName, string verifiedFile);