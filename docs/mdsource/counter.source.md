# Counter

The `Counter` class provides methods to attempt conversion of a `CharSpan` value into a string representation of supported types.

Supported types include 

 such as `Guid`, `DateTimeOffset`, `DateTime`, and, `DateOnly` and `TimeOnly`.

It handles matching equivalent values and assigning a number to each match. It is used by [Scrubbers](/docs/scrubbers.md).


## TryConvert

Takes a CharSpan and attempts to parse it to one of the supported types, then return the tokenized scrubbed value for that value. 

One example usage is inside a custom scrubber

snippet: CounterTryConvert

Results in:

