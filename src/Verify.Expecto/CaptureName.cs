using System.Diagnostics.CodeAnalysis;
using Expecto;
using Expecto.CSharp;

class CaptureName :
    ITestPrinter
{
    static AsyncLocal<string?> local = new();
    
    public static bool TryGet([NotNullWhen(true)] out string? name)
    {
        name = local.Value;
        return name != null;
    }

    public Task BeforeEach(string name)
    {
        local.Value = name;
        return Task.CompletedTask;
    }

    public Task BeforeRun(Test value)
    {
        return Task.CompletedTask;
    }

    public Task Exn(string name, Exception exception, TimeSpan elapsed)
    {
        return Task.CompletedTask;
    }

    public Task Failed(string name, string stackTrace, TimeSpan elapsed)
    {
        local.Value = null;
        return Task.CompletedTask;
    }

    public Task Ignored(string name, string stackTrace)
    {
        return Task.CompletedTask;
    }

    public Task Info(string name)
    {
        return Task.CompletedTask;
    }

    public Task Passed(string name, TimeSpan elapsed)
    {
        local.Value = null;
        return Task.CompletedTask;
    }

    public Task Summary(Impl.ExpectoConfig config, Impl.TestRunSummary summary)
    {
        return Task.CompletedTask;
    }
}