namespace VerifyTests;

/// <summary>
/// Predicate over a line. Return <c>true</c> to remove the line from the output.
/// </summary>
public delegate bool LineFilter(CharSpan line);
