# Members that throw

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