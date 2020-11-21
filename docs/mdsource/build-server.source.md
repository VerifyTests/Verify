# Build Server


## Getting .received files from CI


### AppVeyor

Use a [on_failure build step](https://www.appveyor.com/docs/build-configuration/#build-pipeline) to call [Push-AppveyorArtifact](https://www.appveyor.com/docs/build-worker-api/#push-artifact).

snippet: AppVeyorArtifacts

See also [Pushing artifacts from scripts](https://www.appveyor.com/docs/packaging-artifacts/#pushing-artifacts-from-scripts).


### GitHub Actions

Use a [if: failure()](https://docs.github.com/en/free-pro-team@latest/actions/reference/context-and-expression-syntax-for-github-actions#failure) condition to upload any `*.received.*` files if the build fails.

```yaml
- name: Upload Test Results
  if: failure()
  uses: actions/upload-artifact@v2
  with:
    name: verify-test-results
    path: |
      **/*.received.*
```


## Custom Test directory

In some scenarios, as part of a build, the test assemblies are copied to a different directory or machine to be run. In this case custom code will be required to derive the path to the `.verified.` files. This can be done using [DeriveTestDirectory](naming.md#derivetestdirectory).

For example a possible implementation for [AppVeyor](https://www.appveyor.com/) could be:

snippet: DeriveTestDirectoryAppVeyor
