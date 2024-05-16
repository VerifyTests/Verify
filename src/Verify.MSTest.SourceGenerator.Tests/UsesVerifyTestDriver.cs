namespace VerifyMSTest.SourceGenerator.Tests;

class UsesVerifyTestDriver() :
    TestDriver([new UsesVerifyGenerator().AsSourceGenerator()]);
