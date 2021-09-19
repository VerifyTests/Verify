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

    public Task BeforeEach(string value)
    {
        local.Value = value;
        return Task.CompletedTask;
    }

    public Task BeforeRun(Test value)
    {
        return Task.CompletedTask;
    }

    public Task Exn(string value1, Exception value2, TimeSpan value3)
    {
        return Task.CompletedTask;
    }

    public Task Failed(string value1, string value2, TimeSpan value3)
    {
        local.Value = null;
        return Task.CompletedTask;
    }

    public Task Ignored(string value1, string value2)
    {
        return Task.CompletedTask;
    }

    public Task Info(string value)
    {
        return Task.CompletedTask;
    }

    public Task Passed(string value1, TimeSpan value2)
    {
        local.Value = null;
        return Task.CompletedTask;
    }

    public Task Summary(Impl.ExpectoConfig value1, Impl.TestRunSummary value2)
    {
        return Task.CompletedTask;
    }
}