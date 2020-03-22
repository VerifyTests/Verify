using System.Threading.Tasks;
using VerifyBunit;
using Xunit;
using Xunit.Abstractions;

#region SampleTestBunit
public class SampleTest :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }

    public SampleTest(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endregion