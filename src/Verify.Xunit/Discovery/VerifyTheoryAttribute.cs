using System;
using Xunit.Sdk;

namespace VerifyXunit
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer(nameof(VerifyTheoryDiscoverer), "Verify.Xunit")]
    public class VerifyTheoryAttribute :
        Xunit.TheoryAttribute
    {
    }
}