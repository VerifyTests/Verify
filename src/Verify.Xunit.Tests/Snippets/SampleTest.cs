using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region SampleTest
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