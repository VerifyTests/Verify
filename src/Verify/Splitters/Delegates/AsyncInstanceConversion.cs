using System.Threading.Tasks;

namespace Verify
{
    public delegate Task<ConversionResult> AsyncInstanceConversion<in T>(T target, VerifySettings settings);
    public delegate Task<ConversionResult> AsyncInstanceConversion(object target, VerifySettings settings);
}