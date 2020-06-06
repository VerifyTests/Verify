using System;
using Xunit.Sdk;

namespace VerifyXunit
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer(nameof(TheoryDiscoverer), "Verify.Xunit")]
    public class VerifyTheoryAttribute :
        Xunit.TheoryAttribute
    {
    }
}