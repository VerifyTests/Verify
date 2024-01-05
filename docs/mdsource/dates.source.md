# Dates


By default dates and times (`DateTime`, `DateTimeOffset`, `DateOnly`, and `TimeOnly`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: SerializationTests.ReUseDatetime.verified.txt

To disable this behavior use:


### Instance

snippet: DontScrubDateTimes


### Fluent

snippet: DontScrubDateTimesFluent


### Globally

snippet: DontScrubDateTimesGlobal


## DisableDateCounting

If many calls are made to the the current date/time in quick succession, the date counting behavior (`DateTime_x`) can result in inconsistent results. To revert to the simpler scrubbing convention (`{Scrubbed}`) use DisableDateCounting.


### Instance

snippet: DisableDateCounting


### Fluent

snippet: DisableDateCountingFluent


### Globally

snippet: DisableDateCountingGlobal


## AddExtraDatetimeFormat

`AddExtraDatetimeFormat` allows specifying custom date formats to be scrubbed.

snippet: AddExtraDatetimeFormat


## Inline Dates

Strings containing inline dates can also be scrubbed. There a equivalent APIs for `DateOnly`, `DateTime`, and `DateTimeOffset`.


### Instance

snippet: ScrubInlineDateTimesInstance


### Fluent

snippet: ScrubInlineDateTimesFluent


### Globally

snippet: ScrubInlineDateTimesGlobal


## Named Date and Times

Specific date or times can be named. When any of those values are found, they will be matched with the corresponding name.


### Instance

snippet: NamedDatesAndTimesInstance


### Instance

snippet: NamedDatesAndTimesFluent


### Globally

snippet: NamedDatesAndTimesGlobal