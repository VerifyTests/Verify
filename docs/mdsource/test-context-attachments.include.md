For MSTest and NUnit, Verify will add new received files to the test context's attachments. So for build servers that respect MSTest or NUnit attachment APIs, no changes are required to build configurations.

 * [NUnit Attachments](https://docs.nunit.org/articles/nunit/writing-tests/TestContext.html#addformatter-32)
 * [MSTest TestContext.AddResultFile](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcontext.addresultfile?view=visualstudiosdk-2022#microsoft-visualstudio-testtools-unittesting-testcontext-addresultfile(system-string))