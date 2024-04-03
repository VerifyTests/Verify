[MemoryDiagnoser(false)]
public class ScrubStackTraceBenchmarks
{
    [Benchmark]
    public void ScrubStackTrace() =>
        Scrubbers.ScrubStackTrace(
            """
            Elmah.TestException: This is a test exception that can be safely ignored.
                at Elmah.ErrorLogPageFactory.FindHandler(String name) in C:\ELMAH\src\Elmah\ErrorLogPageFactory.cs:line 126
                at Elmah.ErrorLogPageFactory.GetHandler(HttpContext context, String requestType, String url, String pathTranslated) in C:\ELMAH\src\Elmah\ErrorLogPageFactory.cs:line 66
                at System.Web.HttpApplication.MapHttpHandler(HttpContext context, String requestType, VirtualPath path, String pathTranslated, Boolean useAppConfig)
                at System.Web.HttpApplication.MapHandlerExecutionStep.Execute()
                at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)
            """);
}