static class CounterBuilder
{
    public static Counter Empty() =>
        new(
            true,
            true,
            true,
            false,
#if NET6_0_OR_GREATER
            [],
            [],
#endif
            [],
            [],
            []);
}