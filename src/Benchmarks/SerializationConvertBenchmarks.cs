// The always-on serialization scrub path: as each value is written, VerifyJsonWriter asks the
// Counter to convert it to a stable placeholder. Two shapes:
//  * Typed values (Guid/DateTime/DateTimeOffset/DateOnly/TimeOnly properties) go straight to
//    Counter.TryConvert(value) - a dictionary lookup plus, for a first-seen value, a "Type_N"
//    string allocation. This is where TimeOnly is scrubbed: there is no inline time scrubber.
//  * String values go to Counter.TryConvert(CharSpan), which probes Guid -> DateTimeOffset ->
//    DateTime -> DateOnly -> TimeOnly in turn. The miss case (a plain string that is none of
//    these) runs all five parse attempts and is the tax paid on every serialized string value.
//
// Prevalence from the D:\Code scan (2026-07): DateTime 14.6% of files, Guid 5.1%,
// DateTimeOffset 2.3%, DateOnly 1.6%, TimeOnly 0.27%. Each benchmark converts a batch of 100
// distinct values - one object/collection's worth of work - with a fresh Counter, so the
// per-value allocation of first-seen placeholders is captured and the Counter setup amortised.
// Repeated values (not shown) would be dictionary hits and allocate nothing.
// Tokens are formatted with the same format+culture the probe parses with, so they round-trip.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class SerializationConvertBenchmarks
{
    const int count = 100;
    static readonly CultureInfo culture = CultureInfo.CurrentCulture;

    Guid[] guids = null!;
    DateTime[] dateTimes = null!;
    DateTimeOffset[] dateTimeOffsets = null!;
    DateOnly[] dates = null!;
    TimeOnly[] times = null!;

    string[] plainStrings = null!;
    string[] guidStrings = null!;
    string[] isoStrings = null!;
    string[] shortDateStrings = null!;
    string[] timeStrings = null!;

    [GlobalSetup]
    public void Setup()
    {
        var baseDateTime = new DateTime(2026, 1, 1, 8, 30, 15, DateTimeKind.Unspecified);

        guids = new Guid[count];
        dateTimes = new DateTime[count];
        dateTimeOffsets = new DateTimeOffset[count];
        dates = new DateOnly[count];
        times = new TimeOnly[count];

        plainStrings = new string[count];
        guidStrings = new string[count];
        isoStrings = new string[count];
        shortDateStrings = new string[count];
        timeStrings = new string[count];

        for (var i = 0; i < count; i++)
        {
            guids[i] = new(i + 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dateTimes[i] = baseDateTime.AddMinutes(i);
            dateTimeOffsets[i] = new(dateTimes[i], TimeSpan.Zero);
            dates[i] = DateOnly.FromDateTime(baseDateTime).AddDays(i);
            times[i] = TimeOnly.FromDateTime(baseDateTime).AddMinutes(i);

            plainStrings[i] = $"item {i} description text value";
            guidStrings[i] = guids[i].ToString();
            isoStrings[i] = dateTimes[i].ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK", culture); // caught by the DateTimeOffset probe
            shortDateStrings[i] = dates[i].ToString("d", culture);
            timeStrings[i] = times[i].ToString("h:mm tt", culture);
        }
    }

    // Typed values - the fast path taken for strongly-typed properties

    [Benchmark]
    public void Typed_Guid()
    {
        var counter = Counter.Start();
        foreach (var value in guids)
        {
            counter.TryConvert(value, out _);
        }
    }

    [Benchmark]
    public void Typed_DateTime()
    {
        var counter = Counter.Start();
        foreach (var value in dateTimes)
        {
            counter.TryConvert(value, out _);
        }
    }

    [Benchmark]
    public void Typed_DateTimeOffset()
    {
        var counter = Counter.Start();
        foreach (var value in dateTimeOffsets)
        {
            counter.TryConvert(value, out _);
        }
    }

    [Benchmark]
    public void Typed_Date()
    {
        var counter = Counter.Start();
        foreach (var value in dates)
        {
            counter.TryConvert(value, out _);
        }
    }

    [Benchmark]
    public void Typed_Time()
    {
        var counter = Counter.Start();
        foreach (var value in times)
        {
            counter.TryConvert(value, out _);
        }
    }

    // String values - the probe path taken for every serialized string

    [Benchmark(Baseline = true)]
    public void StringProbe_Miss()
    {
        var counter = Counter.Start();
        foreach (var value in plainStrings)
        {
            counter.TryConvert(value.AsSpan(), out _);
        }
    }

    [Benchmark]
    public void StringProbe_Guid()
    {
        var counter = Counter.Start();
        foreach (var value in guidStrings)
        {
            counter.TryConvert(value.AsSpan(), out _);
        }
    }

    [Benchmark]
    public void StringProbe_Iso()
    {
        var counter = Counter.Start();
        foreach (var value in isoStrings)
        {
            counter.TryConvert(value.AsSpan(), out _);
        }
    }

    [Benchmark]
    public void StringProbe_ShortDate()
    {
        var counter = Counter.Start();
        foreach (var value in shortDateStrings)
        {
            counter.TryConvert(value.AsSpan(), out _);
        }
    }

    [Benchmark]
    public void StringProbe_Time()
    {
        var counter = Counter.Start();
        foreach (var value in timeStrings)
        {
            counter.TryConvert(value.AsSpan(), out _);
        }
    }
}
