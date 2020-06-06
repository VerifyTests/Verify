using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

class Context
{
    static AsyncLocal<Context?> local = new AsyncLocal<Context?>();
    static AsyncLocal<object[]?> dataSet = new AsyncLocal<object[]?>();

    public Context(IMessageSink messageSink, IXunitTestCase testCase)
    {
        MessageSink = messageSink;
        TestCase = testCase;
    }

    public static Context Get()
    {
        if (local.Value == null)
        {
            //TODO:
            throw new Exception("Foo");
        }

        return local.Value!;
    }
    public static object[] GetDataRow()
    {
        if (dataSet.Value == null)
        {
            return Array.Empty<object>();
        }

        return dataSet.Value!;
    }

    public static void Clear()
    {
        local.Value = null;
        dataSet.Value = null;
    }

    public static void Set(IMessageSink messageSink, IXunitTestCase target)
    {
        local.Value = new Context(messageSink, target);
    }

    public static void SetDataRow(object[]? dataRow)
    {
        dataSet.Value = dataRow;
    }

    public IMessageSink MessageSink { get; }
    public IXunitTestCase TestCase { get; }
}