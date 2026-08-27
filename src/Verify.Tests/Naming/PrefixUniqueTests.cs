public class PrefixUniqueTests
{
    // The prefix maps to file names, and two prefixes differing only in case share
    // one set of files on NTFS and APFS
    [Fact]
    public void CaseOnlyDifferenceIsNotUnique()
    {
        PrefixUnique.CheckPrefixIsUnique("PrefixUniqueTests.TheCase");

        var exception = Assert.Throws<Exception>(
            () => PrefixUnique.CheckPrefixIsUnique("prefixuniquetests.thecase"));

        Assert.Contains("The prefix has already been used", exception.Message);
    }

    [Fact]
    public void DistinctPrefixesAreUnique()
    {
        PrefixUnique.CheckPrefixIsUnique("PrefixUniqueTests.Distinct1");
        PrefixUnique.CheckPrefixIsUnique("PrefixUniqueTests.Distinct2");
    }
}
