// These tests mutate global Verify state (DerivePathInfo, UseProjectRelativeDirectory,
// UseSourceFileRelativeDirectory + VerifierSettings.Reset), so they must not run in parallel.
return Runner.RunTestsInAssemblyWithCLIArgs([], ["--sequenced", .. args]);