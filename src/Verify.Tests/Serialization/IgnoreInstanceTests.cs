public class IgnoreInstanceTests
{
    [Fact]
    public Task NonMatchingPredicateKeepsEmptyCollectionsIgnored() =>
        Verify(
            new
            {
                Empty = new List<string>(),
                NotEmpty = new List<string>
                {
                    "TheValue"
                }
            })
            // a predicate that never matches is not a decision to keep the value,
            // so Empty is still ignored as an empty collection
            .IgnoreInstance<List<string>>(_ => false);
}
