namespace VerifyTests;

public delegate Task<CompareResult> StreamCompare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context);