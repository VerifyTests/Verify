namespace VerifyTests;

public static partial class VerifierSettings
{
    static VerifierSettings() =>
        MemberConverter<Exception, string>(
            _ => _.StackTrace,
            (_, value) =>
            {
                if (value == null)
                {
                    return null;
                }

                return Scrubbers.ScrubStackTrace(value);
            });
}