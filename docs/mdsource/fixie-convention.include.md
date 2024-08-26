Fixie is less opinionated than other test frameworks. As such it leaves up to the consumer how to configure test execution.

To enable Verify  the [ITestProject and IExecution interfaces](https://github.com/fixie/fixie/wiki/Customizing-the-Test-Project-Lifecycle#the-default-convention) need to be used.

Requirements:

 * Assign the target assembly in `ITestProject.Configure` using `VerifierSettings.AssignTargetAssembly`
 * Wrap test executions in `IExecution.Run` with a `ExecutionState.Set`

An example implementation of the above:

snippet: TestProject.cs