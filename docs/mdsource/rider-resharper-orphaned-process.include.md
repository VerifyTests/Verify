## Disable orphaned process detection

Resharper and Rider have a feature [Check for orphaned processes spawned by test runner](https://www.jetbrains.com/help/resharper/Reference__Options__Tools__Unit_Testing__Test_Runner.html).

> By default, ReSharper maintains a list of all processes that are launched by the executed tests. If some of theses processes do not exit after the test execution is over, ReSharper will suggest you to terminate the process. If your setup requires some processes started by the tests to continue running, you can clear this checkbox to avoid unnecessary notifications.

Since this project launches diff tools, it will trigger this feature and a dialog will show:

> All unit tests are finished, but child processes spawned by the test runner process are still running. Terminate child process?

<img src="resharper-spawned.png" alt="R# terminate process dialog" width="400">

As such this feature needs to be disabled:


### Disable for solution

Add the following to `[Solution].sln.DotSettings`.

```
<s:String x:Key="/Default/Housekeeping/UnitTestingMru/UnitTestRunner/SpawnedProcessesResponse/@EntryValue">DoNothing</s:String>
```


### Disable for machine


#### Resharper

ReSharper | Options | Tools | Unit Testing | Test Runner

<img src="resharper-ignore-spawned.png" alt="Disable R# orphaned processes detection" width="400">


#### Rider

File | Settings | Manage Layers | This computer | Edit Layer | Build, Execution, Deployment | Unit Testing | Test Runner

<img src="rider-ignore-spawned.png" alt="Disable R# orphaned processes detection" width="500">