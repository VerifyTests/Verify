Directly after the test runner step add a build step to set a flag if the testrunner failed. This is done by using a [failed condition](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/conditions?view=azure-devops&tabs=yaml). This flag will be evaluated in the CopyFiles and PublishBuildArtifacts steps below.

```yaml
- task: CmdLine@2
  displayName: 'Set flag to publish Verify *.received.* files when test step fails'
  condition: failed()
  inputs:
    script: 'echo ##vso[task.setvariable variable=publishverify]Yes'
```

Since the PublishBuildArtifacts step in DevOps does not allow a wildcard it is necessary to need stage the 'received' files before publishing:

```yaml
- task: CopyFiles@2
  condition: eq(variables['publishverify'], 'Yes')
  displayName: 'Copy Verify *.received.* files to Artifact Staging'
  inputs:
    contents: '**\*.received.*' 
    targetFolder: '$(Build.ArtifactStagingDirectory)\Verify'
    cleanTargetFolder: true
    overWrite: true
```

Publish the staged files as a build artifact:

```yaml
- task: PublishBuildArtifacts@1
  displayName: 'Publish Verify *.received.* files as Artifacts'
  name: 'verifypublish'
  condition: eq(variables['publishverify'], 'Yes')
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)\Verify'
    ArtifactName: 'Verify'
    publishLocation: 'Container'
```