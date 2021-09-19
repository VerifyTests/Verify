using Expecto.CSharp;
using Verify;

var config =
    Runner.DefaultConfig
        .UseVerify();
return Runner.RunTestsInAssembly(config, args);