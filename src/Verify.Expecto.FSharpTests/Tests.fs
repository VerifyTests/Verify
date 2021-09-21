module Tests

open Expecto
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
