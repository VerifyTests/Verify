module Tests

open Xunit
open VerifyTests
open VerifyXunit
open System.Threading
open Newtonsoft.Json

VerifierSettings.ModifySerialization(fun t ->
    t.DontScrubDateTimes()
    t.DontIgnoreEmptyCollections()
    t.DontIgnoreFalse())
VerifierSettings.AddExtraSettings(fun t ->
    t.NullValueHandling <- NullValueHandling.Include)

let verify (anything:'T) =
    // Verify doesn't return a Task, exactly, it returns an awaitable.
    // But xunit requires a Task back. In C# you can just await it.
    // I couldn't find a less heavy-handed way of doing the same in F#.
    let awaiter = Verifier.Verify<'T>(anything).GetAwaiter()
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
    [<Fact>]
    let ``None`` () =
        let invalidInt = None
        verify invalidInt

do ()