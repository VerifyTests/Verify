# Serializer settings

Verify uses [Argon](https://github.com/SimonCropp/Argon) for serialization. See [Default Settings](#default-settings) for on how Argon is used and instructions on how to control that usage.

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


## UseUtf8NoBom

The default encoding for snapshot files uses UTF-8 with byte order marks (BOM) enable. To disable UTF-8 BOMs, call `VerifierSettings.UseUtf8NoBom`.

snippet: UseUtf8NoBom


## UseEncoding

To override the encoding used for snapshot files, replacing the default UTF-8 encoding, call `VerifierSettings.UseEncoding` providing a `System.Text.Encoding` instance.

snippet: UseEncoding


## Default settings

Verify uses [Argon](https://github.com/SimonCropp/Argon) for serialization.

> Argon is a JSON framework for .NET. It is a hard fork of Newtonsoft.Json.

See [Argon documentation](https://github.com/SimonCropp/Argon/blob/main/docs/readme.md)

The default `JsonSerializerSettings` are:

snippet: defaultSerialization


### Modify Defaults


#### Globally

snippet: AddExtraSettingsGlobal


#### Instance

snippet: AddExtraSettings


#### Fluent

snippet: AddExtraSettingsFluent


## QuoteName is false

[JsonTextWriter.QuoteName](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteName.htm) is set to false. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences.


## Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior globally use:

snippet: DontIgnoreEmptyCollections


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


#### VerifyJsonWriter

`VerifyJsonWriter` exposes the following members:

 * `Counter` property that gives programmatic access to the counting behavior used by [Guid](guids.md), [Date](dates.md), and [Id](#numeric-ids-are-scrubbed) scrubbing.
 * `Serializer` property that exposes the current `JsonSerializer`.
 * `Serialize(object value)` is a convenience method that calls `JsonSerializer.Serialize` passing in the writer instance and the `value` parameter.
 * `WriteProperty<T, TMember>(T target, TMember value, string name)` method that writes a property name and value while respecting other custom serialization settings eg [member converters](#converting-a-member), [ignore rules](#ignoring-a-type) etc.


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


## Scrub a type

To scrub all members that match a certain type:

snippet: AddScrubType

Or globally:

snippet: AddScrubTypeGlobal

Result:

snippet: SerializationTests.ScrubType.verified.txt


## Ignoring an instance

To ignore instances of a type based on delegate:

snippet: AddIgnoreInstance

Or globally:

snippet: AddIgnoreInstanceGlobal

Result:

snippet: SerializationTests.AddIgnoreInstance.verified.txt


## Scrub a instance

To scrub instances of a type based on delegate:

snippet: AddScrubInstance

Or globally:

snippet: AddScrubInstanceGlobal

Result:

snippet: SerializationTests.AddScrubInstance.verified.txt


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


## Scrub member by expressions

To scrub members of a certain type using an expression:

snippet: ScrubMemberByExpression

Or globally

snippet: ScrubMemberByExpressionGlobal

Result:

snippet: SerializationTests.ScrubMemberByExpression.verified.txt


## Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName

Or globally:

snippet: IgnoreMemberByNameGlobal

Result:

snippet: SerializationTests.IgnoreMemberByName.verified.txt


## Scrub member by name

To scrub members of a certain type using type and name:

snippet: ScrubMemberByName

Or globally:

snippet: ScrubMemberByNameGlobal

Result:

snippet: SerializationTests.ScrubMemberByName.verified.txt


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

This bypasses the Guid and DateTime scrubbing.

Extra types can be added to this mapping:

snippet: TreatAsString


## Converting a member

The value of a member can be mutated before serialization:

snippet: MemberConverter


## SortPropertiesAlphabetically

Serialized properties can optionally be sorted alphabetically, ie ignoring the order they are defined when using reflection.

snippet: SortProperties


## Dictionary sorting

Dictionaries are sorted by key.

To disable use:

snippet: DontSortDictionaries


## Json/JObject sorting

Json and JObject are not sorted.

To enable sorting use:

snippet: SortJsonObjects


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

Then the appended content will be added to the `.verified.txt` file:

snippet: JsonAppenderTests.Stream#00.verified.txt

See [Converters](/docs/converter.md) for more information on `*.00.verified.txt` files.

Examples of extensions using JsonAppenders are [Recorders in Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer#recording) and  [Recorders in Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework#recording).


## See also

 * [Guid behavior](guids.md)
 * [Date behavior](dates.md)