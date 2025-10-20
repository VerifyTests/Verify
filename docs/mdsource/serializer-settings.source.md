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


#### Globally

snippet: UseStrictJsonGlobal


#### Instance

snippet: UseStrictJson


#### Fluent

snippet: UseStrictJsonFluent


#### Result

Then this results in

 * The default `.received.` and `.verified.` extensions for serialized verification to be `.json`.
 * `JsonTextWriter.QuoteChar` to be `"`.
 * `JsonTextWriter.QuoteName` to be `true`.

Then when an object is verified:

snippet: UseStrictJsonVerify

The resulting file will be:

snippet: Tests.Object.verified.json


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


## Changing Argon settings

Extra Argon settings can be made:


### Globally

snippet: ExtraSettingsGlobal


### Instance

snippet: ExtraSettingsInstance


### Argon Converter

One common use case is to register a custom [JsonConverter](https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm). As only writing is required, to help with this there is `WriteOnlyJsonConverter`, and `WriteOnlyJsonConverter<T>`.

For example given the following JsonConverter:

snippet: JsonConverter

It can be added in a ModuleInitializer:

snippet: AddJsonConverter


#### VerifyJsonWriter

A `VerifyJsonWriter` is passed in to the `Write` methods. It exposes context and helper methods to the JsonConverter. For example:

 * `Counter` property that gives programmatic access to the counting behavior used by [Guid](guids.md), [Date](dates.md), and [Id](#numeric-ids-are-scrubbed) scrubbing.
 * `Serializer` property that exposes the current `JsonSerializer`.
 * `Serialize(object value)` is a convenience method that calls `JsonSerializer.Serialize` passing in the writer instance and the `value` parameter.
 * `WriteProperty<T, TMember>(T target, TMember value, string name)` method that writes a property name and value while respecting other custom serialization settings eg [member converters](#converting-a-member), [ignore rules](#ignoring-a-type) etc.


#### Testing JsonConverters

`WriteOnlyJsonConverter` has a `Execute` methods that executes a JsonConverter:

snippet: WriteOnlyJsonConverter_Execute.cs

This can be used to test a JsonConverter.

Given the following JsonConverter:

snippet: JsonConverter

It can be tested with:

snippet: TestJsonConverter

Json converters often have instance level configuration or contextual settings.

snippet: JsonConverterWithSettings

snippet: ConverterSettings

These can be tested:

snippet: TestJsonConverterWithSettingsInstance

snippet: TestJsonConverterWithSettings


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


## Ignore member by predicate

To ignore members of a certain type using a predicate function:

snippet: IgnoreMemberByPredicate

Or globally:

snippet: IgnoreMemberByPredicateGlobal

Result:

snippet: SerializationTests.IgnoreMemberByPredicate.verified.txt


## Scrub member by predicate

To scrub members of a certain type using a predicate function:

snippet: ScrubMemberByPredicate

Or globally:

snippet: ScrubMemberByPredicateGlobal

Result:

snippet: SerializationTests.ScrubMemberByPredicate.verified.txt


## Converting a member

The value of a member can be mutated before serialization:

snippet: MemberConverter


## See also

 * [Obsolete members](/docs/obsolete-members.md)
 * [Type to string mapping](/docs/type-to-string-mapping.md)
 * [Guids](/docs/guids.md)
 * [Dates](/docs/dates.md)
 * [Scrubbing](/docs/scrubbers.md)
 * [Members that throw](/docs/members-throw.md)
 * [Ordering](/docs/ordering.md)
 * [Encoding](/docs/encoding.md)
 * [JsonAppender](/docs/jsonappender.md)
