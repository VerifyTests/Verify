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

### Azure DevOps YAML Pipeline
Directly after the test runner step add a build step to set a flag if the testrunner failed.  This is done by using a [failed condition](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/conditions?view=azure-devops&tabs=yaml).  This flag will be evaluated in the CopyFiles and 
PublishBuildArtifacts steps below.

```yaml
- task: CmdLine@2
  displayName: 'Set flag to publish received files when previous step fails'
  condition: failed()
  inputs:
    script: 'echo ##vso[task.setvariable variable=publishverify]Yes'
```

Since the PublishBuildArtifacts step in DevOps does not allow a wildcard we need stage the 'received' files before publishing them:

```yaml
- task: CopyFiles@2
  condition: eq(variables['publishverify'], 'Yes')
  displayName: 'Copy received files to Artifact Staging'
  inputs:
    contents: '**\*.received.*' 
    targetFolder: '$(Build.ArtifactStagingDirectory)\Verify'
    cleanTargetFolder: true
    overWrite: true
```
Finally publish the staged files as a build artifact:

```yaml
- task: PublishBuildArtifacts@1
  displayName: 'Publish received files as Artifacts'
  name: 'verifypublish'
  condition: eq(variables['publishverify'], 'Yes')
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)\Verify'
    ArtifactName: 'Verify'
    publishLocation: 'Container'
``` 



## Custom directory and file name

In some scenarios, as part of a build, the test assemblies are copied to a different directory or machine to be run. In this case custom code will be required to derive the path to the `.verified.` files. This can be done using [DerivePathInfo](naming.md#derivepathinfo).

For example a possible implementation for [AppVeyor](https://www.appveyor.com/) could be:

snippet: DerivePathInfoAppVeyor
