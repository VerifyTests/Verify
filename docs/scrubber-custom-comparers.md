# Add custom comparer to a scrubber

In some cases you want to override default scrubber comparers.

```csharp
private class StubGuidComparer : IEqualityComparer<Guid>
{
    public bool Equals(Guid x, Guid y) => true;

    public int GetHashCode(Guid obj) => 1;
}

[Fact]
public async Task ShouldInjectCustomGuidComparer()
{
    var obj = new
    {
        Item1 = Guid.NewGuid(),
        Item2 = Guid.NewGuid(),
        Item3 = Guid.NewGuid()
    };

    var settings = new VerifySettings();
    settings.ReplaceScrubberGuidComparer(new StubGuidComparer());
    await Verify(obj, settings: settings);
}
```

This test will create such `.verified.txt` file

```json
{
  Item1: Guid_1,
  Item2: Guid_1,
  Item3: Guid_1
}
```

Available comperers replacements are:

```csharp
void ReplaceScrubberDateTimeOffsetComparer(IEqualityComparer<DateTimeOffset> comparer)
void ReplaceScrubberGuidComparer(IEqualityComparer<Guid> comparer)
void ReplaceScrubberDateTimeComparer(IEqualityComparer<DateTime> comparer)
void ReplaceScrubberTimeComparer(IEqualityComparer<Time> comparer)
void ReplaceScrubberDateComparer(IEqualityComparer<Date> comparer)
```