// Used for the combinations name cache. Each input list is materialized once, so a key
// recurs as the same boxed instance and reference equality is what makes the cache hit.
// Value equality would be wrong there: DateTime.Equals ignores Kind and
// DateTimeOffset.Equals compares only the instant, while both render into the name.
sealed class ReferenceComparer :
    IEqualityComparer<object>
{
    public static ReferenceComparer Instance = new();

    public new bool Equals(object? x, object? y) =>
        ReferenceEquals(x, y);

    public int GetHashCode(object value) =>
        RuntimeHelpers.GetHashCode(value);
}
