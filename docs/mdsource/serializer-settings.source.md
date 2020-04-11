# Serializer settings

Serialization settings can be customized at three levels:

 * Method: Will run the verification in the current test method.
 * Class: Will run for all verifications in all test methods for a test class.
 * Global: Will run for test methods on all tests.

toc


## Default settings

The default serialization settings are:

snippet: defaultSerialization


## Single quotes used

[JsonTextWriter.QuoteChar](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteChar.htm) is set to single quotes `'`. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences.


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

To disable this behavior globally use:

snippet: DontScrubGuids


## Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: SerializationTests.ShouldReUseDatetime.verified.txt

To disable this behavior globally use:

snippet: DontScrubDateTimes


## Default Booleans are ignored

By default values of `bool` and `bool?` are ignored during verification. So properties that equate to 'false' will not be written,

To disable this behavior globally use:

snippet: DontIgnoreFalse


## Change defaults at the verification level

`DateTime`, `DateTimeOffset`, `Guid`, `bool`, and empty collection behavior can also be controlled at the verification level: 

snippet: ChangeDefaultsPerVerification


## Changing settings globally

To change the serialization settings for all verifications use `Global.ApplyExtraSettings()`:

snippet: ExtraSettings


## Scoped settings

snippet: ScopedSerializer

Result:

snippet: Verify.Xunit.Tests/VerifyObjectSamples.ScopedSerializer.verified.txt


## Ignoring a type

To ignore all members that match a certain type:

snippet: AddIgnoreType

Result:

snippet: Tests.IgnoreType.verified.txt


## Ignoring a instance

To ignore instances of a type based on delegate:

snippet: AddIgnoreInstance

Result:

snippet: Tests.AddIgnoreInstance.verified.txt


## Obsolete members ignored

Members with an [ObsoleteAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.obsoleteattribute) are ignored:

snippet: WithObsoleteProp

Result:

snippet: Tests.WithObsoleteProp.verified.txt


### Including Obsolete members

Obsolete members can be included using `IncludeObsoletes`:

snippet: WithObsoletePropIncluded

Result:

snippet: Tests.WithObsoletePropIncluded.verified.txt



## Ignore member by expressions

To ignore members of a certain type using an expression:

snippet: IgnoreMemberByExpression

Result:

snippet: Tests.IgnoreMemberByExpression.verified.txt


## Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName

Result:

snippet: Tests.IgnoreMemberByName.verified.txt


## Members that throw

Members that throw exceptions can be excluded from serialization based on the exception type or properties.

By default members that throw `NotImplementedException` or `NotSupportedException` are ignored.

Note that this is global for all members on all types.

Ignore by exception type:

snippet: IgnoreMembersThatThrow

Result:

snippet: Tests.CustomExceptionProp.verified.txt

Ignore by exception type and expression:

snippet: IgnoreMembersThatThrowExpression

Result:

snippet: Tests.ExceptionMessageProp.verified.txt


## DisableNewLineEscaping

By default newlines in json are escaped with `\r\n`:

snippet: NewLineEscapedInProperty

snippet: SerializationTests.NewLineEscapedInProperty.verified.txt

This can be disabled:

snippet: DisableNewLineEscaping

snippet: SerializationTests.NewLineNotEscapedInProperty.verified.txt