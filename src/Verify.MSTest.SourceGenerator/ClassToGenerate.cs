/// <summary>
/// The data model for the class that is annotated with [UsesVerify] and needs to be generated.
/// </summary>
/// <remarks>
/// For incremental source generator caching to work properly, this class must _fully_ implement
/// value (not reference) equality.
///
/// The built in equality and hash code implementations won't work because this type includes a
/// collection (which has reference equality semantics), so we must implement them ourselves.
/// </remarks>
readonly record struct ClassToGenerate(string? Namespace, string ClassName, ParentClass[] ParentClasses)
{
    public string? Namespace { get; } = Namespace;
    public string ClassName { get; } = ClassName;
    public ParentClass[] ParentClasses { get; } = ParentClasses;

    public bool Equals(ClassToGenerate other) =>
        Namespace == other.Namespace &&
        ClassName == other.ClassName &&
        ParentClasses.SequenceEqual(other.ParentClasses);

    public override int GetHashCode()
    {
        // Overflow is fine, just wrap
        unchecked
        {
            var hash = 1430287;
            hash = hash * 7302013 ^ (Namespace ?? string.Empty).GetHashCode();
            hash = hash * 7302013 ^ ClassName.GetHashCode();

            // Include (up to) the last 8 elements in the hash code to balance performance and specificity.
            // The runtime also does this for structural equality; see
            // https://github.com/dotnet/runtime/blob/2c39e052327302cafaea652e4b29dd6855e9572a/src/libraries/System.Private.CoreLib/src/System/Array.cs#L755-L768.
            var length = ParentClasses.Length;
            for (var i = length >= 8 ? length - 8 : 0; i < length; i++)
            {
                hash = hash * 7302013 ^ ParentClasses[i]
                    .GetHashCode();
            }

            return hash;
        }
    }
}