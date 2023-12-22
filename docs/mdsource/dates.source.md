# Dates


By default dates and times (`DateTime`, `DateTimeOffset`, `DateOnly`, and `TimeOnly`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: SerializationTests.ShouldReUseDatetime.verified.txt

To disable this behavior use:


### Instance

snippet: DontScrubDateTimes


### Fluent

snippet: DontScrubDateTimesFluent


### Globally

snippet: DontScrubDateTimesGlobal


## AddExtraDatetimeFormat

`AddExtraDatetimeFormat` allows specifying custom date formats to be scrubbed.

snippet: AddExtraDatetimeFormat


## Inline Dates

Strings containing inline dates can also be scrubbed. To enable this behavior, use:


### Instance

snippet: ScrubInlineDates


### Fluent

snippet: ScrubInlineDatesFluent


### Globally

snippet: ScrubInlineDatesGlobal


## Named Date and Times

Specific date or times can be named. When any of those values are found, they will be matched with the corresponding name.


### Instance

snippet: NamedDatesAndTimesInstance


### Instance

snippet: NamedDatesAndTimesFluent


### Globally

snippet: NamedDatesAndTimesGlobal