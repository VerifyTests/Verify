[assembly: Parallelize]
// Without this every verification fails with "TestContext is null". Applied to the assembly rather
// than the test class so that building this project also covers the generator skipping the static
// [TestClass] in DanglingSnapshots.cs, which an assembly wide attribute also reaches.
[assembly: UsesVerify]
