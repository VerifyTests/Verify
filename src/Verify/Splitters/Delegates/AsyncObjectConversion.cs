using System.Threading.Tasks;

namespace Verify
{
    public delegate Task<ConversionResult> AsyncObjectConversion<in T>(T target, VerifySettings settings);
    public delegate Task<ConversionResult> AsyncObjectConversion(object target, VerifySettings settings);
}