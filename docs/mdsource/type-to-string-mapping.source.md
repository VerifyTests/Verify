# Type to string mapping

Certain types, **when passed directly to `Verify()`**, are written directly without going through json serialization.

The API for controlling this behavior is `TreatAsString()`

Example of passing directly to `Verify()`:

```cs
[Fact]
public Task Example() =>
    Verify(new DateOnly(2020, 10, 4));
```


## Default type mapping

The default mapping is:

snippet: typeToStringMapping


## Scrubbing is bypassed

This approach bypasses the Guid and DateTime scrubbing.


## DateTime formatting

How DateTimes are converted to a string:

snippet: DateFormatter_DateTime.cs


## DateTimeOffset formatting

How DateTimeOffset are converted to a string:

snippet: DateFormatter_DateTimeOffset.cs


## Override TreatAsString defaults

The default TreatAsString behavior can be overridden:

snippet: OverrideTreatAsStringDefaults


## Extra Types

Extra types can be added to this mapping:

snippet: TreatAsString


## Redundant json serialization settings

Since this approach bypasses json serialization, any json serialization settings are redundant. For example `DontScrubDateTimes`, `UseStrictJson`, and `DontScrubGuids`.

Note that any json serialization settings will still apply to anything amended to the target via [Recording](docs/recording.md) or [JsonAppenders](jsonappender.md)


## See also

 * [Serializer settings](/docs/serializer-settings.md)
 * [Guids](/docs/guids.md)
 * [Dates](/docs/dates.md)