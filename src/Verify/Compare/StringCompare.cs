namespace VerifyTests;

public delegate Task<CompareResult> StringCompare(string received, string verified, IReadOnlyDictionary<string, object> context);