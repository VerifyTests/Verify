Use a [on_failure build step](https://www.appveyor.com/docs/build-configuration/#build-pipeline) to call [Push-AppveyorArtifact](https://www.appveyor.com/docs/build-worker-api/#push-artifact).

```
on_failure:
  - ps: Get-ChildItem *.received.* -recurse | % { Push-AppveyorArtifact $_.FullName -FileName $_.Name }
```

See also [Pushing artifacts from scripts](https://www.appveyor.com/docs/packaging-artifacts/#pushing-artifacts-from-scripts).