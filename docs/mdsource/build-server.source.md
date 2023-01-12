# Build Server


## Getting .received files from CI


### AppVeyor

include: build-server-appveyor


### GitHub Actions

include: build-server-githubactions


### Azure DevOps YAML Pipeline

include: build-server-azuredevops


## Custom directory and file name

In some scenarios, as part of a build, the test assemblies are copied to a different directory or machine to be run. In this case custom code will be required to derive the path to the `.verified.` files. This can be done using [DerivePathInfo](naming.md#derivepathinfo).

For example a possible implementation for [AppVeyor](https://www.appveyor.com/) could be:

snippet: DerivePathInfoAppVeyor