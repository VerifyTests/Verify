namespace VerifyTests;

/// <summary>
/// Per-line transform. Return <c>null</c> to drop the line, or the replacement
/// content to keep (return the input as a string to keep unchanged).
/// </summary>
public delegate string? LineReplace(CharSpan line);
