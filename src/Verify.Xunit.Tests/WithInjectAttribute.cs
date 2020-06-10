using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[InjectInfo]
public class WithInjectAttribute
{
    [Fact]
    public Task Test()
    {
        return Verifier.Verify("Result");
    }
}