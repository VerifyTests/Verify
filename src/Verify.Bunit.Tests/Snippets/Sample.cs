using System.Threading.Tasks;
using VerifyBunit;
using Xunit;
using Xunit.Abstractions;

#region SampleTestBunit
public class Sample :
    VerifyBase
{
    [Fact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }

    public Sample(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endregion