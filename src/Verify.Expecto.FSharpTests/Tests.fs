module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
  testList "samples" [
    testAsync "firstTest" {
      do! Verifier.Verify<string>("samples_firstTest", "value1") |> Async.AwaitTask
    }
    testAsync "secondTest" {
      do! Verifier.Verify<string>("samples_secondTest", "value2") |> Async.AwaitTask
    }
  ]
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
    testAsync "uniqueTests" {
      let settings = new VerifySettings()
      settings.UniqueForRuntime()
      do! Verifier.Verify<string>("uniqueTests", "value1", settings) |> Async.AwaitTask
    }
// end-snippet