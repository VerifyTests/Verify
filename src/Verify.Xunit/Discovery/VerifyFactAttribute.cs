using System;
using Xunit.Sdk;

namespace VerifyXunit
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer(nameof(FactDiscoverer), "Verify.Xunit")]
    public class VerifyFactAttribute :
        Xunit.FactAttribute
    {
    }
}