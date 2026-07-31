# Default values

To keep snapshots concise, Verify omits members whose value is equal to the default for their type. For example a `null` reference, a `0`, or a `default(DateTime)` are not written to the snapshot.

This is controlled by [Argon](https://github.com/SimonCropp/Argon)'s [DefaultValueHandling](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_DefaultValueHandling.htm), which Verify defaults to `Ignore`:

snippet: defaultSerialization


## Defaults are omitted

Given an object with a member left at its default (`null`):

snippet: NullDefault

That member is omitted from the result:

snippet: SerializationTests.NullProperty.verified.txt


## Booleans

Non-nullable `bool` members are an exception: `false` is rendered even though it is the default for `bool`. This keeps the presence or absence of a boolean explicit in the snapshot. Nullable `bool?` members still follow the standard behavior, so a `null` is omitted while `false` is rendered.

Given the following model:

snippet: BoolModel

When both members are left at their defaults:

snippet: BoolDefault

The non-nullable `bool` renders as `false`, while the `null` `bool?` is omitted:

snippet: SerializationTests.BoolDefault.verified.txt

When the nullable member is explicitly set to `false`:

snippet: BoolFalse

Both members are rendered:

snippet: SerializationTests.BoolFalse.verified.txt


## Enums

Enums are rendered by name. As with any other type, an enum member equal to the default for its type is omitted. The default for an enum is the member with the value `0`, so a non-nullable enum sitting on that member is dropped from the snapshot. Unlike `bool`, enums are not force-rendered.

Given the following model:

snippet: EnumModel

When the non-nullable member is left at its default (the `0` member `Apple`), and the nullable member is set to that same value:

snippet: EnumDefault

The non-nullable `Fruit` is omitted, while the nullable member holding the same value is rendered by name. Its default is `null`, so `Apple` differs from the default and is kept:

snippet: SerializationTests.EnumDefault.verified.txt

When both members are set to a non-default value:

snippet: EnumSet

Both are rendered:

snippet: SerializationTests.EnumSet.verified.txt


## Including default values

Default handling can be overridden so that all members are rendered, including those at their default value. To also render `null` members, [NullValueHandling](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_NullValueHandling.htm) needs to be set to `Include` as well.


### Fluent

snippet: IncludeDefaultValues

Result:

snippet: SerializationTests.NullPropertyInclude.verified.txt


### Globally

snippet: IncludeDefaultValuesGlobal


## Empty collections

By default empty collections and dictionaries are also omitted. This can be disabled:

snippet: DontIgnoreEmptyCollections

See [Serializer settings](/docs/serializer-settings.md#empty-collections-are-ignored).


## See also

 * [Serializer settings](/docs/serializer-settings.md)
 * [Scrubbing](/docs/scrubbers.md)
 * [Ordering](/docs/ordering.md)
