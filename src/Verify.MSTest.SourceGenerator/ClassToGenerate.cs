/// <summary>
/// The data model for the class that is annotated with [UsesVerify] and needs to be generated.
/// </summary>
/// <remarks>
/// For incremental source generator caching to work properly, this class must _fully_ implement
/// value (not reference) equality.
///
/// The built in equality and hash code implementations won't work because this type includes a
/// collection (which has reference equality semantics), so we must implement them ouselves.
/// </remarks>
readonly record struct ClassToGenerate
{
    public string? Namespace { get; }
    public string ClassName { get; }
    public ParentClass[] ParentClasses { get; }

    public ClassToGenerate(string? @namespace, string className, ParentClass[] parentClasses)
    {
        Namespace = @namespace;
        ClassName = className;
        ParentClasses = parentClasses;
    }

    public readonly bool Equals(ClassToGenerate other) =>
        Namespace == other.Namespace &&
        ClassName == other.ClassName &&
        ParentClasses.SequenceEqual(other.ParentClasses);

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hash = 1430287;
            hash = hash * 7302013 ^ (Namespace ?? string.Empty).GetHashCode();
            hash = hash * 7302013 ^ ClassName.GetHashCode();

            // Include (up to) the last 8 elements in the hash code to balance performance and specificity.
            // The runtime also does this for structural equality; see
            // https://github.com/dotnet/runtime/blob/2c39e052327302cafaea652e4b29dd6855e9572a/src/libraries/System.Private.CoreLib/src/System/Array.cs#L755-L768.
            for (var i = (ParentClasses.Length >= 8 ? ParentClasses.Length - 8 : 0); i < ParentClasses.Length; i++)
            {
                hash = hash * 7302013 ^ ParentClasses[i].GetHashCode();
            }
            return hash;
        }
    }
}
