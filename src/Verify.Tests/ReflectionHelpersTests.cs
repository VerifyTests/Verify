using System.Collections.Immutable;

public class ReflectionHelpersTests
{
    [Theory]
    [InlineData(typeof(string), typeof(object), true)]
    [InlineData(typeof(object), typeof(string), false)]
    [InlineData(typeof(object), typeof(object), true)]
    [InlineData(typeof(string), typeof(string), true)]
    [InlineData(typeof(string), typeof(ICloneable), true)]
    [InlineData(typeof(string), typeof(IComparable<string>), true)]
    [InlineData(typeof(string), typeof(IComparable<int>), false)]
    [InlineData(typeof(string), typeof(IComparable<>), true)]
    [InlineData(typeof(Dictionary<int, string>), typeof(IDictionary), true)]
    [InlineData(typeof(Dictionary<int, string>), typeof(IDictionary<int, string>), true)]
    [InlineData(typeof(Dictionary<int, string>), typeof(IDictionary<string, string>), false)]
    [InlineData(typeof(IDictionary<int, string>), typeof(IDictionary<string, string>), false)]
    [InlineData(typeof(Dictionary<int, string>), typeof(IDictionary<,>), true)]
    [InlineData(typeof(IDictionary<int, string>), typeof(IDictionary<,>), true)]
    [InlineData(typeof(IDictionary<int, string>), typeof(ICollection<KeyValuePair<int, string>>), true)]
    [InlineData(typeof(IDictionary<int, string>), typeof(ICollection<>), true)]
    public void InheritsFrom(Type type, Type parent, bool match) =>
        Assert.Equal(match, type.InheritsFrom(parent));

    [Theory]
    [MemberData(nameof(EmptyCollectionOrDictionaryTestCases))]
    public void IsEmptyCollectionOrDictionary(object o, bool isEmpty) =>
        Assert.Equal(isEmpty, o.IsEmptyCollectionOrDictionary());

    public static TheoryData<object, bool> EmptyCollectionOrDictionaryTestCases()
    {
        ImmutableArray<int> uninitializedImmutableArray = default;

        var cases = new TheoryData<object, bool>
        {
            { Array.Empty<int>(), true },
            { new ImmutableArray<int>(), true },
            { uninitializedImmutableArray, true },
            { ImmutableDictionary.Create<byte, string>(), true },
            { new Dictionary<string, object>(), true}
        };
        return cases;
    }
}