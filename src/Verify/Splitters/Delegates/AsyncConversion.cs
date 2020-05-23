using System.Threading.Tasks;

namespace Verify
{
    public delegate Task<ConversionResult> AsyncConversion<in T>(T target, VerifySettings settings);
    public delegate Task<ConversionResult> AsyncConversion(object target, VerifySettings settings);
}