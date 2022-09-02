public class MatchingFileFinderTests
{
    [Theory]
    [MemberData(nameof(GetData))]
    public void Simple(string fileNamePrefix, string suffix, string file, bool result) =>
        Assert.Equal(result, MatchingFileFinder.ShouldInclude(fileNamePrefix, suffix, file));

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Test.Method", ".verified", @"C:/Path/Test.Method.verified.txt", true};
        yield return new object[] {"Test.Method", ".verified", @"C:/Path/Test.Method.00name1.verified.txt", true};
        yield return new object[] {"Test.Method", ".verified", @"C:/Path/Test.Method.00.name1.verified.txt", true};
        yield return new object[] {"Test.Method1", ".verified", @"C:/Path/Test.Method2.00.name1.verified.txt", false};
        yield return new object[] {"Test1.Method", ".verified", @"C:/Path/Test2.Method.00.name1.verified.txt", false};
    }
}