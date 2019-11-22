---
name: Bug fix
about: Create a bug fix to help us improve
---

Note: New issues raised, where it is clear the submitter has not read the issue template, are likely to be closed with "please read the issue template". Please don't take offense at this. It is simply a time management decision. If someone raises an issue, and can't be bothered to spend the time to read the issue template, then the project maintainers should not be expected to spend the time to read the submitted issue. Often too much time is spent going back and forth in issue comments asking for information that is outlined in the issue template.


#### Preamble

General questions may be better placed [StackOveflow](https://stackoverflow.com/).

Where relevant, ensure you are using the current stable versions on your development stack. For example:

 * Visual Studio
 * [.NET SDK or .NET Core SDK](https://www.microsoft.com/net/download)
 * Any related NuGet packages

Any code or stack traces must be properly formatted with [GitHub markdown](https://guides.github.com/features/mastering-markdown/).


#### Describe the bug

A clear and concise description of what the bug is. Include any relevant version information.

A clear and concise description of what you expected to happen.

Add any other context about the problem here.


#### Minimal Repro

Ensure you have replicated the bug in a minimal solution with the fewest moving parts. Often this will help point to the true cause of the problem. Upload this repro as part of the issue, preferably a public GitHub repository or a downloadable zip. The repro will allow the maintainers of this project to smoke test the any fix.

#### Submit a PR that fixes the bug

Submit a [Pull Request (PR)](https://help.github.com/articles/about-pull-requests/) that fixes the bug. Include in this PR a test that verifies the fix. If you were not able to fix the bug, a PR that illustrates your partial progress will suffice.