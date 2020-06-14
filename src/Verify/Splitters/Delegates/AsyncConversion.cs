using System.Threading.Tasks;

namespace VerifyTesting
{
    public delegate Task<ConversionResult> AsyncConversion<in T>(T target, VerifySettings settings);
    public delegate Task<ConversionResult> AsyncConversion(object target, VerifySettings settings);
}