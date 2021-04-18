module Tests

open Xunit
open VerifyTests
open VerifyXunit
open Newtonsoft.Json

// begin-snippet: NullValueHandling
VerifierSettings.AddExtraSettings(fun settings ->
    settings.NullValueHandling <- NullValueHandling.Include)
// end-snippet

[<UsesVerify>]
module Tests =
// begin-snippet: FsTest
    [<Fact>]
    let ``My test`` () =
        async {
            let settings = Verifier.Verify 15
            do! settings.ToTask() |> Async.AwaitTask
        }
// end-snippet
do ()