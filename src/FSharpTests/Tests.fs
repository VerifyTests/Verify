module Tests

open Xunit
open VerifyTests
open VerifyXunit
open Argon

// begin-snippet: DefaultValueHandling
VerifierSettings.AddExtraSettings(fun settings -> settings.DefaultValueHandling <- DefaultValueHandling.Include)
// end-snippet

// begin-snippet: FsTest
[<Fact>]
let MyTest () =
     Verifier.Verify(15).ToTask() |> Async.AwaitTask
// end-snippet

// begin-snippet: WithSettings
[<Fact>]
let WithFluentSetting () =
    Verifier
        .Verify(15)
        .UseMethodName("customName")
        .ToTask()
    |> Async.AwaitTask
// end-snippet
do ()