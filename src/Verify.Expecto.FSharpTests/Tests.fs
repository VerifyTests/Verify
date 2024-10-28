module Tests
// begin-snippet: SampleTestExpecto
open Expecto
open VerifyTests
open VerifyExpecto

[<Tests>]
let tests =
    testTask "findPerson" {
        let person = ClassBeingTested.FindPerson()
        do! Verifier.Verify("findPerson", person)
    }
// end-snippet


// begin-snippet: UniqueForSampleExpecto
[<Tests>]
let uniqueTests =
    testTask "unique" {
        let settings = VerifySettings()
                           .UniqueForRuntime()
        do! Verifier.Verify("unique", "value", settings)
    }
// end-snippet

[<Tests>]
let typeNameTests =
    testTask "typeNameTests" {
        let settings = VerifySettings()
                           .UseTypeName("CustomTypeName")
        do! Verifier.Verify("typeNameTests", "Value", settings)
    }

[<Tests>]
let methodNameTests =
    testTask "methodNameTests" {
        let settings = VerifySettings()
                           .UseMethodName("CustomMethodName")
        do! Verifier.Verify("methodNameTests", "Value", settings)
    }