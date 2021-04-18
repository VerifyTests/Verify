module Tests

open Xunit
open VerifyXunit
open System.Threading
open Newtonsoft.Json
open System.Reflection

let verify (anything:'T) =
    // Verify doesn't return a Task, exactly, it returns an awaitable.
    // But xunit requires a Task back. In C# you can just await it.
    // I couldn't find a less heavy-handed way of doing the same in F#.
    let awaiter = Verifier.Verify<'T>(anything)
                    .UseDirectory("Verified")
                    .ModifySerialization(fun t ->
                        t.DontScrubDateTimes()
                        t.DontIgnoreEmptyCollections()
                        t.DontIgnoreFalse())
                    .AddExtraSettings(fun t ->
                        t.NullValueHandling <- NullValueHandling.Include)
                    .GetAwaiter()
    async {
        use handle = new SemaphoreSlim(0)
        awaiter.OnCompleted(fun () -> ignore (handle.Release()))
        let! _ = handle.AvailableWaitHandle |> Async.AwaitWaitHandle
        return awaiter.GetResult()
    } |> Async.StartAsTask

[<UsesVerify>]
module Tests =
    [<Fact>]
    let ``My test`` () =
        verify 15

// without this, attribute Verify refuses to work.
// Also, it automatically replaces anything that looks like the value with {ProjectDirectory},
// which we also never want.
[<AssemblyMetadataAttribute("Verify.ProjectDirectory", "anything that is unlikely to show up in values")>]
do ()