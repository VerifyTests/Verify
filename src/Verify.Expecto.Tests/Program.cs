using Expecto.CSharp;
using VerifyExpecto;

var config =
    Runner.DefaultConfig
        .UseVerify();
return Runner.RunTestsInAssembly(config, args);