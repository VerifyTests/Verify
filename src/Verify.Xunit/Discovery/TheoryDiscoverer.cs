using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using Options=Xunit.Abstractions.ITestFrameworkDiscoveryOptions;

class VerifyTheoryDiscoverer :
    TheoryDiscoverer
{
    IMessageSink sink;

    public VerifyTheoryDiscoverer(IMessageSink sink)
        : base(sink)
    {
        this.sink = sink;
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
        Options options,
        ITestMethod method,
        IAttributeInfo attribute,
        object[] row)
    {
        return base.CreateTestCasesForDataRow(options, method, attribute, row)
            .Select(x => new TestCase((XunitTestCase)x, true));
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
        Options options,
        ITestMethod method,
        IAttributeInfo attribute)
    {
        var xunitTestCases = base.CreateTestCasesForTheory(options, method, attribute).ToList();
        return xunitTestCases
            .Select(x => new TestCase((XunitTestCase)x, false));
    }
}