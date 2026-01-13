public class UserMachineScrubberTests :
    BaseTest
{
    public UserMachineScrubberTests() =>
        UserMachineScrubber.ResetReplacements("FakeMachineName", "FakeUserName");

    [Fact]
    public void MultipleChunks()
    {
        var builder = new StringBuilder(capacity: 8, maxCapacity: int.MaxValue);
        builder.Append("FakeUserName");
        using var counter = Counter.Start();
        UserMachineScrubber.PerformReplacements(builder, "FakeUserName", "TheUserName");
        Assert.Equal("TheUserName", builder.ToString());
    }

    [Fact]
    public Task ScrubBoth() =>
        Verify("FakeUserName FakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserName() =>
        Verify("FakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidBefore() =>
        Verify(" FakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidAfter() =>
        Verify("FakeUserName ")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidWrapped() =>
        Verify(" FakeUserName ")
            .ScrubUserName();

    [Fact]
    public Task ScrubMachineName() =>
        Verify("FakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidBefore() =>
        Verify(" FakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidAfter() =>
        Verify("FakeMachineName ")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidWrapped() =>
        Verify(" FakeMachineName ")
            .ScrubMachineName();

    [Fact]
    public Task ScrubUserNameInValidBefore() =>
        Verify("AFakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameInValidAfter() =>
        Verify("FakeUserNameA")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameInValidWrapped() =>
        Verify("AFakeUserNameA")
            .ScrubUserName();

    [Fact]
    public Task ScrubMachineNameInValidBefore() =>
        Verify("AFakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInValidAfter() =>
        Verify("FakeMachineNameA")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInValidWrapped() =>
        Verify("AFakeMachineNameA")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInQuotes() =>
        Verify("\"FakeMachineName\"")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInJson() =>
        Verify(" \"Message\": \"Machine name: FakeMachineName\",")
            .ScrubMachineName();

    [Fact]
    public Task ScrubUserNameInQuotes() =>
        Verify("\"FakeUserName\"")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameInJson() =>
        Verify(" \"Message\": \"User name: FakeUserName\",")
            .ScrubUserName();
}