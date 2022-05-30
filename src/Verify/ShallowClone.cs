static class ShallowClone
{
    public static List<T> Clone<T>(this List<T> original) =>
        new(original);
}