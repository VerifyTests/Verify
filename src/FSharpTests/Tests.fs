[<VerifyXunit.UsesVerify>]
module Tests

open Xunit
open VerifyTests
open VerifyXunit
open Newtonsoft.Json

// begin-snippet: NullValueHandling
VerifierSettings.AddExtraSettings(fun settings ->
  settings.NullValueHandling <- NullValueHandling.Include)
// end-snippet

// begin-snippet: FsTest
[<Fact>]
let MyTest () =
  async {
    do! Verifier.Verify(15)
          .ToTask() |> Async.AwaitTask
  }
// end-snippet

// begin-snippet: WithSettings
[<Fact>]
let WithFluentSetting () =
  async {
    do! Verifier.Verify(15)
          .UseMethodName("customName")
          .ToTask() |> Async.AwaitTask
  }
// end-snippet
do ()