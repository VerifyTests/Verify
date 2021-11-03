# Serializer settings

Serialization settings can be customized at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.


## Not valid json

Note that the output is technically not valid json.

 * Names and values are not quoted.
 * Newlines are not escaped.

The reason for these is that it makes approval files cleaner and easier to read and visualize/understand differences.


### UseStrictJson

To use strict json call `VerifierSettings.UseStrictJson`:

snippet: UseStrictJson

Then this result in 

 * The default `.received.` and `.verified.` extensions for serialized verification to be `.json`.
 * `JsonTextWriter.QuoteChar` to be `"`.
 * `JsonTextWriter.QuoteName` to be `true`.

Then when an object is verified:

snippet: UseStrictJsonVerify

The resulting file will be:

snippet: Tests.Object.verified.json


## Default settings

The default `JsonSerializerSettings` are:

snippet: defaultSerialization


### Modify Defaults


#### Globally

snippet: AddExtraSettingsGlobal


#### On Settings

snippet: AddExtraSettings


#### On Settings Fluent

snippet: AddExtraSettingsFluent


## QuoteName is false

[JsonTextWriter.QuoteName](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteName.htm) is set to false. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences.


## Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior globally use:

snippet: DontIgnoreEmptyCollections


## Guids are scrubbed

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

Results in the following:

snippet: SerializationTests.ShouldReUseGuid.verified.txt

Strings containing inline Guids can also be scrubbed. To enable this behavior, use:

snippet: ScrubInlineGuids


### Disable

To disable this behavior use:

snippet: DontScrubGuids

Or with the fluent api:

snippet: DontScrubGuidsFluent

To disable this behavior globally use:

snippet: DontScrubGuidsGlobal


## Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: SerializationTests.ShouldReUseDatetime.verified.txt

To disable this behavior use:

snippet: DontScrubDateTimes

Or using the fluent api use:

snippet: DontScrubDateTimesFluent

Or globally use:

snippet: DontScrubDateTimesGlobal


## Numeric Ids are scrubbed

By default numeric properties (`int`, `uint`, `long`, `ulong`, and their nullable counterparts) suffixed with `Id` are sanitized during verification. This is done by finding each id and taking a counter based that that specific id. That counter is then used replace the id values. This allows for repeatable tests when id are bing generated.

snippet: NumericId

Results in the following:

snippet: SerializationTests.NumericIdScrubbing.verified.txt


### Convention

A custom "is member a numeric Id" convention is supported.

To define a convention use:

snippet: TreatAsNumericId

To define a convention globally use:

snippet: TreatAsNumericIdGlobal


### Disable

To disable this globally use:

snippet: DisableNumericId

To disable this behavior globally use:

snippet: DisableNumericIdGlobal


## Default Booleans are ignored

By default values of `bool` and `bool?` are ignored during verification. So properties that equate to 'false' will not be written,

To disable this behavior globally use:

snippet: DontIgnoreFalse


## Change defaults at the verification level

`DateTime`, `DateTimeOffset`, `Guid`, `bool`, and empty collection behavior can also be controlled at the verification level: 

snippet: ChangeDefaultsPerVerification


## Changing Json.NET settings

Extra Json.NET settings can be made:


### Globally

snippet: ExtraSettingsGlobal


### Instance

snippet: ExtraSettingsInstance


### Json.NET Converter

One common use case is to register a custom [JsonConverter](https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm). As only writing is required, to help with this there is `WriteOnlyJsonConverter`, and `WriteOnlyJsonConverter<T>`.

snippet: CompanyConverter

snippet: JsonConverter


## Scoped settings

snippet: ScopedSerializer

Result:

snippet: Verify.Xunit.Tests/VerifyObjectSamples.ScopedSerializer.verified.txt


## Ignoring a type

To ignore all members that match a certain type:

snippet: AddIgnoreType

Or globally:

snippet: AddIgnoreTypeGlobal

Result:

snippet: SerializationTests.IgnoreType.verified.txt


## Ignoring a instance

To ignore instances of a type based on delegate:

snippet: AddIgnoreInstance

Or globally:

snippet: AddIgnoreInstanceGlobal

Result:

snippet: SerializationTests.AddIgnoreInstance.verified.txt


## Obsolete members ignored

Members with an [ObsoleteAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.obsoleteattribute) are ignored:

snippet: WithObsoleteProp

Result:

snippet: SerializationTests.WithObsoleteProp.verified.txt


### Including Obsolete members

Obsolete members can be included using `IncludeObsoletes`:

snippet: WithObsoletePropIncluded

Or globally:

snippet: WithObsoletePropIncludedGlobally

Result:

snippet: SerializationTests.WithObsoletePropIncluded.verified.txt


## Ignore member by expressions

To ignore members of a certain type using an expression:

snippet: IgnoreMemberByExpression

Or globally

snippet: IgnoreMemberByExpressionGlobal

Result:

snippet: SerializationTests.IgnoreMemberByExpression.verified.txt


## Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName

Or globally:

snippet: IgnoreMemberByNameGlobal

Result:

snippet: SerializationTests.IgnoreMemberByName.verified.txt


## Members that throw

Members that throw exceptions can be excluded from serialization based on the exception type or properties.

By default members that throw `NotImplementedException` or `NotSupportedException` are ignored.

Note that this is global for all members on all types.

Ignore by exception type:

snippet: IgnoreMembersThatThrow

Or globally:

snippet: IgnoreMembersThatThrowGlobal

Result:

snippet: SerializationTests.CustomExceptionProp.verified.txt

Ignore by exception type and expression:

snippet: IgnoreMembersThatThrowExpression

Or globally:

snippet: IgnoreMembersThatThrowExpressionGlobal

Result:

snippet: SerializationTests.ExceptionMessageProp.verified.txt


## TreatAsString

Certain types, when passed directly in to Verify, are written directly without going through json serialization.

The default mapping is:

snippet: typeToStringMapping

This bypasses the Guid and DateTime scrubbing mentioned above.

Extra types can be added to this mapping:

snippet: TreatAsString


## Converting a member

The value of a member can be mutated before serialization:

snippet: MemberConverter

This can also be configured globally:

snippet: MemberConverterGlobal


## SortPropertiesAlphabetically

Serialized properties can optionally be sorted alphabetically, ie ignoring the order they are defined when using reflection.

snippet: SortProperties


## JsonAppender

A JsonAppender allows extra content (key value pairs) to be optionally appended to the output being verified. JsonAppenders can use the current context to determine what should be appended or if anything should be appended.

Register a JsonAppender:

snippet: RegisterJsonAppender

When when content is verified:

snippet: JsonAppender

The content from RegisterJsonAppender will be included in the output:

snippet: JsonAppenderTests.WithJsonAppender.verified.txt

If the target is a stream or binary file:

snippet: JsonAppenderStream

Then the appended content will be added to the `*.00.verified.txt` file:

snippet: JsonAppenderTests.Stream.00.verified.txt

See [Converters](/docs/converter.md) for more information on `*.00.verified.txt` files.

Examples of extensions using JsonAppenders are [Recorders in Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording) and  [Recorders in Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording).