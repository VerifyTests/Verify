using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task<ConversionResult> AsyncConversion<in T>(T target, VerifySettings settings);
    public delegate Task<ConversionResult> AsyncConversion(object target, VerifySettings settings);
}