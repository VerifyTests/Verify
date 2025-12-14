namespace VerifyTests;

partial class InnerVerifier
{
    static IEnumerable<Target> emptyTargets = [];

    public Task<VerifyResult> Verify() =>
        VerifyInner(null, null, emptyTargets, true);

    public Task<VerifyResult> Verify(object? target) =>
        VerifyInner(target, null, [new(target)], true);
}