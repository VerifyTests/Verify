module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
    testTask "findPerson" {
        let person = ClassBeingTested.FindPerson()
        do! Verify("findPerson", person)
    }
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
    testTask "unique" {
        let settings = new VerifySettings()
        settings.UniqueForRuntime()
        do! Verify("unique", "value1", settings)
    }
// end-snippet
