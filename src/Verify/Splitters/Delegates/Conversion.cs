using System.Collections.Generic;

namespace VerifyTests
{
    public delegate ConversionResult Conversion<in T>(T target, VerifySettings settings, IReadOnlyDictionary<string, object> context);

    public delegate ConversionResult Conversion(object target, VerifySettings settings, IReadOnlyDictionary<string, object> context);
}