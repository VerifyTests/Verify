using System.Diagnostics;
using System.Threading.Tasks;
using VerifyXunit;

[UsesVerify]
public class Inherited : Base
{
    public override Task TestToOverride()
    {
        Trace.WriteLine("");
        return base.TestToOverride();
    }
}