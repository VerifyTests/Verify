module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
  testAsync "findPerson" {
    let person = ClassBeingTested.FindPerson();
    do! Verifier.Verify("findPerson", person) |> Async.AwaitTask
  }
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
  testAsync "unique" {
    let settings = new VerifySettings()
    settings.UniqueForRuntime()
    do! Verifier.Verify("unique", "value1", settings) |> Async.AwaitTask
  }
// end-snippet