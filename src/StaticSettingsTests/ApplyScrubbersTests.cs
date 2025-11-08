public class ApplyScrubbersTests :
    BaseTest
{
    public ApplyScrubbersTests() =>
        Scrubbers.ResetReplacements("FakeMachineName", "FakeUserName");

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
}