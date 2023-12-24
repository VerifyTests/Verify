[UsesVerify]
public class DateTimeScrubbingTests :
    BaseTest
{
    public DateTimeScrubbingTests() =>
        VerifierSettings.ScrubInlineDateTimes("F");

    [Fact]
    public Task Test()
    {
        var dateTime = DateTime.Now;
        return Verify($"a {dateTime:F} b");
    }
}