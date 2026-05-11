namespace VerifyTests;

/// <summary>
/// Per-line transform.
/// </summary>
/// <param name="line">The line content (no trailing newline).</param>
/// <param name="replacement">
/// When the return value is <c>true</c>, the content to emit. Assign
/// <paramref name="line" /> itself to keep the line unchanged with zero allocation.
/// </param>
/// <returns>
/// <c>false</c> to drop the line entirely; <c>true</c> to keep it.
/// </returns>
public delegate bool LineReplace(CharSpan line, out CharSpan replacement);
