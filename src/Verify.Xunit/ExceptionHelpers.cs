using System;
using System.Reflection;
using Xunit.Sdk;

static class ExceptionHelpers
{
    static FieldInfo fieldInfo;

    static ExceptionHelpers()
    {
        var type = typeof(EqualException);
        fieldInfo = type.GetField("message", BindingFlags.Instance | BindingFlags.NonPublic);
        if (fieldInfo == null)
        {
            throw new Exception($"Could not find 'message' field on {nameof(EqualException)}.");
        }
    }

    public const string CommandHasBeenCopiedToTheClipboard = "Verification command has been copied to the clipboard.";
    public static void PrefixWithCopyCommand(this EqualException exception)
    {
        fieldInfo.SetValue(exception, $@"{CommandHasBeenCopiedToTheClipboard}
{exception.Message}");
    }
}