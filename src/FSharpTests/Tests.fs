module Tests

open Xunit
open VerifyTests
open VerifyXunit
open Newtonsoft.Json

VerifierSettings.ModifySerialization(fun t ->
    t.DontScrubDateTimes()
    t.DontIgnoreEmptyCollections()
    t.DontIgnoreFalse())
VerifierSettings.AddExtraSettings(fun t ->
    t.NullValueHandling <- NullValueHandling.Include)

[<UsesVerify>]
module Tests =
    [<Fact>]
    let ``My test`` () =
        async {
            let settings = Verifier.Verify 15
            do! settings.ToTask() |> Async.AwaitTask
        }
    [<Fact>]
    let ``None`` () =
        async {
            let invalidInt = None

            let settings = Verifier.Verify invalidInt
            do! settings.ToTask() |> Async.AwaitTask
        }

do ()