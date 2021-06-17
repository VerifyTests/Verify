# Snapshot testing compared to traditional assertion

  * **Less test code**: Snapshot tests require less code to write.
  * **Reduced risk of incorrect test code**: Given the above assertion based test it would be difficult to ensure that no property is missing from the assertion. For example if a new property is added to the model. In the snapshot test that change would automatically be highlighted when the test is next run.
  * **Test failure visualization**: Snapshot test allows [visualization in a diff tool](https://github.com/VerifyTests/DiffEngine) that works for [complex models](/docs/SecondDiff.png) and [binary documents](/docs/binary.md).
  * **Multiple changes visualized in singe test run**: In the assertion approach, if multiple assertions require changing, this only becomes apparent over multiple test runs. In the snapshot approach, multiple changes can be [visualized in one test run](/docs/SecondDiff.png).
  * **Simpler creation of test "contract"**: In the assertion approach, complex models can require significant code to do the initial assertion. In the snapshot approach, the actual test and code-under-test can be used to create that "contract". See [initial verification](#initial-verification).
  * **Snapshot files committed to source control**: All resulting snapshot files are committed to source control in the most appropriate format. This means these files can be viewed at any time using any tooling. The files can also be diff'd over the history of the code base. This works for any file type, for example:
    * Html content can be committed as `.html` files.
    * Office documents can be committed as a rendered `.png` (see [Verify.Aspose](https://github.com/VerifyTests/Verify.Aspose)).
    * Database schema can be committed as `.sql` (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).


## Example

Given the following method:


### Class being tested

snippet: ClassBeingTested


### Tests

Compare a traditional assertion based test to a verification test.


#### Traditional assertion test:

snippet: TraditionalTest


#### Snapshot test

snippet: SnapshotTest

Produces a snapshot file `SnapshotTest.verified.txt`:

snippet: CompareToAssert.SnapshotTest.verified.txt