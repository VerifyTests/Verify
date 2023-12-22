# Guids

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

Results in the following:

snippet: SerializationTests.ShouldReUseGuid.verified.txt

Strings containing inline Guids can also be scrubbed. To enable this behavior, use:


### Instance

snippet: ScrubInlineGuids

### Fluent

snippet: ScrubInlineGuidsFluent

### Globally

snippet: ScrubInlineGuidsGlobal


## Disable

To disable this behavior use:

### Instance

snippet: DontScrubGuids


### Fluent

snippet: DontScrubGuidsFluent


### Globally

snippet: DontScrubGuidsGlobal


## Named Guid

Specific Guids can be named. When any of those Guids are found, it will be replaced with the supplied name.


### Instance

snippet: NamedGuidInstance


### Instance

snippet: NamedGuidFluent


### Globally

snippet: NamedGuidGlobal