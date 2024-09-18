# Add custom comparer to a scrubber

In some cases you want to override default scrubber comparers.

Available comparers replacements are:

```csharp
void ReplaceScrubberDateTimeOffsetComparer(IEqualityComparer<DateTimeOffset> comparer)
void ReplaceScrubberGuidComparer(IEqualityComparer<Guid> comparer)
void ReplaceScrubberDateTimeComparer(IEqualityComparer<DateTime> comparer)
void ReplaceScrubberTimeComparer(IEqualityComparer<Time> comparer)
void ReplaceScrubberDateComparer(IEqualityComparer<Date> comparer)
```

Example with `Guid`:

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

```text
{
  Item1: Guid_1,
  Item2: Guid_1,
  Item3: Guid_1
}
```

Example with `DateTimeOffset` - comparison of Date part only:

```csharp
private class StubDateTimeOffsetComparer : IEqualityComparer<DateTimeOffset>
{
    public bool Equals(DateTimeOffset x, DateTimeOffset y) => x.Date == y.Date;

    public int GetHashCode(DateTimeOffset obj) => 1;
}

[Fact]
public async Task ShouldInjectCustomDateTimeOffsetComparer()
{
    var obj = new
    {
        Item1 = new DateTimeOffset(2023, 5, 1, 11, 5, 2, TimeSpan.FromHours(1)),
        Item2 = new DateTimeOffset(2023, 5, 1, 12, 3, 1, TimeSpan.FromHours(1)),
    };

    var settings = new VerifySettings();
    settings.ReplaceScrubberDateTimeOffsetComparer(new StubDateTimeOffsetComparer());
    await Verify(obj, settings: settings);
}
```

This test will create such `.verified.txt` file

```text
{
  Item1: DateTimeOffset_1,
  Item2: DateTimeOffset_1
}
```