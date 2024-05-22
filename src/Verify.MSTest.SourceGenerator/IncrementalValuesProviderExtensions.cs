static class IncrementalValuesProviderExtensions
{
    public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(this IncrementalValuesProvider<TSource?> source) where TSource : struct =>
        source.Where(x => x.HasValue).Select((item, _) => item!.Value);

    public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(this IncrementalValuesProvider<TSource?> source) =>
        source.Where(x => x is not null).Select((item, _) => item!);
}
