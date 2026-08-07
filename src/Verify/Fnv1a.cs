// FNV-1a. Used instead of string.GetHashCode since that is randomized per process, and callers
// need names that are stable across runs.
static class Fnv1a
{
    public static string Hash(string value)
    {
        var hash = 14695981039346656037UL;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= 1099511628211UL;
        }

        return hash.ToString("x16");
    }
}
