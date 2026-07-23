# Guids

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

Results in the following:

snippet: SerializationTests.ReUseGuid.verified.txt


## Disable

To disable this behavior use:


### Instance

snippet: DontScrubGuids


### Fluent

snippet: DontScrubGuidsFluent


### Globally

snippet: DontScrubGuidsGlobal


## Inline Guids

Strings containing inline Guids can also be scrubbed. Guids in the `D` format (`00000000-0000-0000-0000-000000000000`) and the `N` format (`00000000000000000000000000000000`) are matched when bounded by non-alphanumeric characters. `B` and `P` formatted Guids are covered by the `D` format matching between their delimiters. Note that any 32 character hex sequence is a valid `N` format Guid, so hex content of that exact length (an MD5 hash for example) will also be scrubbed.

To enable this behavior, use:


### Instance

snippet: ScrubInlineGuidsInstance


### Fluent

snippet: ScrubInlineGuidsFluent


### Globally

snippet: ScrubInlineGuidsGlobal


## Named Guid

Specific Guids can be named. When any of those Guids are found, it will be replaced with the supplied name.


### Instance

snippet: NamedGuidInstance


### Instance

snippet: NamedGuidFluent


### Globally

snippet: NamedGuidGlobal


### Inferred Name

The name can be inferred from the variable name by omitting the name argument:

snippet: InferredNamedGuidFluent

Result: 

snippet: GuidScrubberTests.InferredNamedGuidFluent.verified.txt
