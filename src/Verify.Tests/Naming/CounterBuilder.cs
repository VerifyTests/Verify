static class CounterBuilder
{
    public static Counter Empty() =>
        new(
            true,
#if NET6_0_OR_GREATER
            [],
            [],
#endif
            [],
            [],
            []);
}