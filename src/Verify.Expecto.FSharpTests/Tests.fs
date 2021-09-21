module Tests

open Expecto
open VerifyExpecto

[<Tests>]
let tests =
  testList "samples" [
    testAsync "first" {
      do! Verifier.Verify<string>("theName", "value") |> Async.AwaitTask
    }
    testAsync "second" {
      do! Verifier.Verify<string>("theName2", "value2") |> Async.AwaitTask
    }
  ]
