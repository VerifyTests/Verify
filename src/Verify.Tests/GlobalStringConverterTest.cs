using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Verify.Tests
{
    [UsesVerify]
    public class GlobalStringConverterTest
    {
        [Fact]
        public async Task Should_Respect_Global_String_Converter_Table()
        {
            VerifierSettings.TreatAsString<Address>((target, verifySettings) => target.Country);
            
            var person = ClassBeingTested.FindPerson();
            await Verifier.Verify(person);
        }
    }
}