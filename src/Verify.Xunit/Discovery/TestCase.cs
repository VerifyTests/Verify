using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

class TestCase :
    IXunitTestCase
{
    XunitTestCase target = null!;
    private readonly bool isTheory;

    public TestCase(XunitTestCase target, bool isTheory)
    {
        this.target = target;
        this.isTheory = isTheory;
    }

    public TestCase()
    {
    }

    public void Deserialize(IXunitSerializationInfo info)
    {
        var targetType = info.GetValue<string>("targetType");
        target = (XunitTestCase) Activator.CreateInstance(Type.GetType(targetType));
        target.Deserialize(info);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue("targetType", target.GetType().AssemblyQualifiedName);
        target.Serialize(info);
    }

    public string DisplayName => target.DisplayName;

    public string SkipReason => target.SkipReason;

    public ISourceInformation SourceInformation
    {
        get => target.SourceInformation;
        set => target.SourceInformation = value;
    }

    public ITestMethod TestMethod => target.TestMethod;

    public object[] TestMethodArguments => target.TestMethodArguments;

    public Dictionary<string, List<string>> Traits => target.Traits;

    public string UniqueID => target.UniqueID;

    public async Task<RunSummary> RunAsync(IMessageSink messageSink, IMessageBus messageBus, object[] arguments, ExceptionAggregator aggregator, CancellationTokenSource cancellation)
    {
        Context.Set(messageSink, this);
        try
        {
            if (isTheory)
            {
                var theoryTestCaseRunner = new VerifyTheoryTestCaseRunner(this, target.DisplayName, target.SkipReason, arguments, messageSink, messageBus, aggregator, cancellation);
                return await theoryTestCaseRunner.RunAsync();
            }
            return await target.RunAsync(messageSink, messageBus, arguments, aggregator, cancellation);
        }
        finally
        {
            Context.Clear();
        }
    }

    public Exception InitializationException => target.InitializationException;

    public IMethodInfo Method => target.Method;

    public int Timeout => target.Timeout;
    public object[]? DataRow { get; set; }
}