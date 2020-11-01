using System.Collections.Generic;
using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task<ConversionResult> AsyncConversion<in T>(T target, VerifySettings settings, IReadOnlyDictionary<string, object> context);

    public delegate Task<ConversionResult> AsyncConversion(object target, VerifySettings settings, IReadOnlyDictionary<string, object> context);
}