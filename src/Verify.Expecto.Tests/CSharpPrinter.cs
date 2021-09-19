using Expecto;
using Expecto.CSharp;

public class CSharpPrinter : ITestPrinter
{
    Task ITestPrinter.BeforeEach(string value)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.BeforeRun(Test value)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Exn(string value1, Exception value2, TimeSpan value3)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Failed(string value1, string value2, TimeSpan value3)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Ignored(string value1, string value2)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Info(string value)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Passed(string value1, TimeSpan value2)
    {
        return Task.CompletedTask;
    }

    Task ITestPrinter.Summary(Impl.ExpectoConfig value1, Impl.TestRunSummary value2)
    {
        return Task.CompletedTask;
    }
}