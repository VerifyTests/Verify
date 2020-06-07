using System;
using System.Threading;
using Xunit.Abstractions;

class Context
{
    static AsyncLocal<Context?> local = new AsyncLocal<Context?>();

    public Context(IMessageSink sink, TestCase testCase)
    {
        Sink = sink;
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

    public static void Clear()
    {
        local.Value = null;
    }

    public static void Set(IMessageSink sink, TestCase target)
    {
        local.Value = new Context(sink, target);
    }

    public IMessageSink Sink { get; }
    public TestCase TestCase { get; }
}