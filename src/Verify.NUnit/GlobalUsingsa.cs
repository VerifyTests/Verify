static class Extensions
{
    public static bool TryGetParent(this TestAdapter adapter, [NotNullWhen(true)] out ITest? parent)
    {
        var test = adapter.GetTest();
        parent = test.Parent;
        if (parent is ParameterizedMethodSuite methodSuite)
        {
            parent = methodSuite.Parent;
        }

        return parent != null;
    }

#if NET8_0_OR_GREATER

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_test")]
    public static extern Test GetTest(this TestAdapter adapter);

#else

    static FieldInfo testField = typeof(TestAdapter).GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static Test GetTest(this TestAdapter adapter) =>
        (Test) testField.GetValue(adapter)!;

#endif

}
