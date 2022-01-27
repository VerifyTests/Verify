public class ReflectionHelpersTests
{
    [Theory]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(ICollection), true)]
    [InlineData(typeof(IList), true)]
    [InlineData(typeof(IEnumerable), false)]
    [InlineData(typeof(ICollection<int>), true)]
    [InlineData(typeof(IList), true)]
    [InlineData(typeof(IEnumerable<int>), false)]
    public void ShouldNotLock(Type type, bool isCollection)
    {
        Assert.Equal(isCollection, type.IsCollection());
    }
}