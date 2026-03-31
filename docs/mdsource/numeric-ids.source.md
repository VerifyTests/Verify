# Numeric Ids


## ScrubNumericIds

Opt in scrubbing of numeric properties ending in `Id` or `ID`. Each unique numeric value gets a stable counter based replacement (`Id_1`, `Id_2`, etc), similar to [Guid](guids.md) and [Date](dates.md) scrubbing.


### Fluent

snippet: ScrubNumericIdsFluent

Results in the following:

snippet: SerializationTests.ScrubNumericIdsFluent.verified.txt


### Instance

snippet: ScrubNumericIdsInstance


### Globally

snippet: ScrubNumericIdsGlobal


## ScrubMembers approach

For more targeted control, `ScrubMembers` can be used to check the DeclaringType and the name of the member.

snippet: NumericIdSample

Produces

snippet: NumericIdSample.Test.verified.txt
