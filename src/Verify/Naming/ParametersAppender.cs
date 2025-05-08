namespace VerifyTests;

public delegate Action<StringBuilder> ParametersAppender(IReadOnlyDictionary<string, object?> values, Counter counter);