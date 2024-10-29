module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
    testTask "findPerson" {
        let person = ClassBeingTested.FindPerson()
        do! Verifier.Verify("findPerson", person).ToTask()
    }
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
    testTask "unique" {
        let settings = VerifySettings()
        settings.UniqueForRuntime()
        do! Verifier.Verify("unique", "value", settings).ToTask()
    }
// end-snippet

[<Tests>]
let typeNameTests =
    testTask "typeNameTests" {
        let settings = VerifySettings()
        settings.UseTypeName("CustomTypeName")
        do! Verifier.Verify("typeNameTests", "Value", settings).ToTask()
    }

[<Tests>]
let methodNameTests =
    testTask "methodNameTests" {
        let settings = VerifySettings()
        settings.UseMethodName("CustomMethodName")
        do! Verifier.Verify("methodNameTests", "Value", settings).ToTask()
    }