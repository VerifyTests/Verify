# Build Server


## Getting .received files from CI


### AppVeyor

Use an [on_failure build step](https://www.appveyor.com/docs/build-configuration/#build-pipeline) to call [Push-AppveyorArtifact](https://www.appveyor.com/docs/build-worker-api/#push-artifact).

snippet: AppVeyorArtifacts

See also [Pushing artifacts from scripts](https://www.appveyor.com/docs/packaging-artifacts/#pushing-artifacts-from-scripts).


## Custom Test directory

In some scenarios, as part of a build, the test assemblies are copied to a different directory or machine to be run. In this case custom code will be required to derive the path to the `.verified.` files. This can be done using a custom delegate via `SharedVerifySettings.DeriveTestDirectory`. The parameters passed are as follows:

 * `type`: The test type.
 * `testDirectory`: The directory that the test source file existed in at compile time.
 * `projectDirectory`: The directory that the project existed in at compile time.

For example a possible implementation for [AppVeyor](https://www.appveyor.com/) could be:

snippet: DeriveTestDirectory