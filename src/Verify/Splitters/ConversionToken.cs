namespace VerifyTests;

/// <summary>
/// One conversion that named a source, and so the identity shared by that source and every target
/// the conversion derived from it.
/// </summary>
/// <remarks>
/// Compared by reference: two conversions of two attachments are two tokens, however alike their
/// targets are. <see cref="Parent" /> is the conversion that produced the target this one
/// converted, which is how a page of a pdf that was itself rendered from a docx is traced back to
/// the docx.
/// </remarks>
sealed class ConversionToken(ConversionToken? parent)
{
    public ConversionToken? Parent { get; } = parent;
}
