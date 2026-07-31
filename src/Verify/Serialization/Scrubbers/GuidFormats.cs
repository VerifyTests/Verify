namespace VerifyTests;

/// <summary>
/// The <see cref="Guid" /> formats that <see cref="VerifySettings.ScrubInlineGuids(GuidFormats)" /> matches.
/// </summary>
[Flags]
public enum GuidFormats
{
    /// <summary>
    /// The "D" format: 32 hex digits separated by hyphens (e.g. <c>00000000-0000-0000-0000-000000000000</c>).
    /// The "B" and "P" formats are covered by this since they wrap the "D" format in delimiters.
    /// </summary>
    Dashed = 1,

    /// <summary>
    /// The "N" format: 32 hex digits with no separators (e.g. <c>00000000000000000000000000000000</c>).
    /// Note that any 32 character hex sequence (an MD5 hash for example) is a valid "N" format Guid, so
    /// content of that exact length will also be scrubbed.
    /// </summary>
    Undashed = 2,

    /// <summary>
    /// Both <see cref="Dashed" /> and <see cref="Undashed" />.
    /// </summary>
    All = Dashed | Undashed
}
