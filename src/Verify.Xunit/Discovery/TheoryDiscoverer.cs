using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using Options=Xunit.Abstractions.ITestFrameworkDiscoveryOptions;

class TheoryDiscoverer :
    Xunit.Sdk.TheoryDiscoverer
{
    IMessageSink messageSink;

    public TheoryDiscoverer(IMessageSink messageSink)
        : base(messageSink)
    {
        this.messageSink = messageSink;
    }

    public override IEnumerable<IXunitTestCase> Discover(
        Options options,
        ITestMethod method,
        IAttributeInfo attribute)
    {
        var xunitTestCases = base.Discover(options, method, attribute).ToList();
        return xunitTestCases
            .Select(x => new TestCase(x));
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(Options options, ITestMethod method, IAttributeInfo attribute, object[] row)
    {
        Context.SetDataRow(row);
        return base.CreateTestCasesForDataRow(options, method, attribute, row)
            .Select(x => new TestCase(x));
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(Options options, ITestMethod method, IAttributeInfo attribute)
    {
        var xunitTestCases = base.CreateTestCasesForTheory(options, method, attribute).ToList();
        return xunitTestCases
            .Select(x => new TestCase(x));
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForSkip(Options options, ITestMethod method, IAttributeInfo attribute, string skipReason)
    {
        return base.CreateTestCasesForSkip(options, method, attribute, skipReason)
            .Select(x => new TestCase(x));
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForSkippedDataRow(Options options, ITestMethod method, IAttributeInfo attribute, object[] row, string skipReason)
    {
        Context.SetDataRow(row);
        return base.CreateTestCasesForSkippedDataRow(options, method, attribute, row, skipReason)
            .Select(x => new TestCase(x));
    }
}