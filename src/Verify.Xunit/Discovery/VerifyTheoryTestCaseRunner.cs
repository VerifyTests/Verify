using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

class VerifyTheoryTestCaseRunner :
    XunitTheoryTestCaseRunner
{
    public VerifyTheoryTestCaseRunner(IXunitTestCase test, string name, string skip, object[] arguments, IMessageSink sink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellation) :
        base(test, name, skip, arguments, sink, messageBus, aggregator, cancellation)
    {
    }

    protected override XunitTestRunner CreateTestRunner(ITest test, IMessageBus bus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] methodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> attributes, ExceptionAggregator aggregator, CancellationTokenSource cancellation)
    {
        return new VerifyXunitTestRunner(test, bus, testClass, constructorArguments, testMethod, methodArguments, skipReason, attributes, new ExceptionAggregator(aggregator), cancellation);
    }
}

class VerifyXunitTestRunner :
    XunitTestRunner
{
    public VerifyXunitTestRunner(ITest test, IMessageBus bus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] methodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellation) :
        base(test, bus, testClass, constructorArguments, testMethod, methodArguments, skipReason, beforeAfterAttributes, aggregator, cancellation)
    {
    }

    protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
    {
        var testCase = (TestCase) TestCase;
        testCase.DataRow = TestMethodArguments;
        return base.InvokeTestMethodAsync(aggregator);
    }
}