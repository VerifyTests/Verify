#pragma warning disable VerifyDanglingSnapshots

var result = Runner.RunTestsInAssemblyWithCLIArgs([], args);

DanglingSnapshots.Run();

return result;
