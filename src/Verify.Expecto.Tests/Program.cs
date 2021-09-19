using Expecto.CSharp;
var config =
    Runner.DefaultConfig
        .AddPrinter(new CSharpPrinter())
        .AddNUnitSummary("bin/Expecto.Tests.CSharp.TestResults.xml")
        .AddJUnitSummary("bin/Expecto.Tests.CSharp.TestResults.junit.xml");
return Runner.RunTestsInAssembly(config, args);