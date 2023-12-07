namespace VerifyTests.ExceptionParsing;

class ParseException :
    Exception
{
    const string exceptionSuffix = " Ensure the current stable of Verify is being used.";

    public ParseException(string message)
        :
        base($"{message}{exceptionSuffix}")
    {
    }

    public ParseException(string message, Exception inner)
        :
        base($"{message}{exceptionSuffix}", inner)
    {
    }
}