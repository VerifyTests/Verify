namespace VerifyTests;

public delegate Task<CompareResult> StringCompare(string received, string verified, IVerifySettings settings);