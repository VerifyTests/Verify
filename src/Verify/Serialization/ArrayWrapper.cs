namespace VerifyTests;

public class ArrayWrapper<T> : IEnumerable
    where T : IEnumerable
{
    IEnumerable inner;

    public ArrayWrapper(IEnumerable inner) =>
        this.inner = inner;

    public IEnumerator GetEnumerator() =>
        inner.GetEnumerator();
}