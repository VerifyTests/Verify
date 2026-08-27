// User code can reach Counter.Current from parallel work inside one test, so the
// value caches have to survive concurrent access
public class CounterConcurrencyTests
{
    [Fact]
    public void ConcurrentGuids()
    {
        using var counter = Counter.Start();

        var guids = Enumerable.Range(0, 500)
            .Select(_ => Guid.NewGuid())
            .ToList();

        var names = new ConcurrentBag<string>();
        Parallel.ForEach(guids, guid => names.Add(counter.NextString(guid)));

        // one name per distinct input, and no name handed out twice
        Assert.Equal(guids.Count, names.Distinct().Count());

        // and the same input keeps the name it was given
        foreach (var guid in guids)
        {
            Assert.Contains(counter.NextString(guid), names);
        }

        Assert.Equal(guids.Count, names.Distinct().Count());
    }

    [Fact]
    public void ConcurrentNumericIds()
    {
        using var counter = Counter.Start();

        var ids = Enumerable.Range(0, 500)
            .Select(_ => (long) _)
            .ToList();

        var names = new ConcurrentBag<string>();
        Parallel.ForEach(ids, id => names.Add(counter.NextNumericIdString("TheEntity", id)));

        Assert.Equal(ids.Count, names.Distinct().Count());

        foreach (var id in ids)
        {
            Assert.Contains(counter.NextNumericIdString("TheEntity", id), names);
        }

        Assert.Equal(ids.Count, names.Distinct().Count());
    }
}
