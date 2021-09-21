module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
  testAsync "findPersonTest" {
    let person = ClassBeingTested.FindPerson();
    do! Verifier.Verify("findPersonTest", person) |> Async.AwaitTask
  }
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
  testAsync "uniqueTests" {
    let settings = new VerifySettings()
    settings.UniqueForRuntime()
     do! Verifier.Verify("uniqueTests", "value1", settings) |> Async.AwaitTask
  }
// end-snippet