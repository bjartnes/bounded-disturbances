module Tests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp
open NBomber.Http.FSharp
open NBomber.Http.FSharp.Http

[<Fact>]
let ``XUnit test`` () =

    let step1 = HttpStep.create ("simple step", (fun (_sc: StepContext<unit> ) -> Http.createRequest "GET" "https://localhost:5001/weatherforecast"))

    // v 0.16 (latest) has a weird, undocumented assert syntax
    // v 0.17 should fix this again
    // https://github.com/PragmaticFlow/NBomber/issues/184
    // https://github.com/PragmaticFlow/NBomber/blob/v0.16.0/examples/CSharp/CSharp.Examples.NUnit/Tests.cs

    let assertions = [
       Assertion.forStep("simple step", fun stats -> stats.OkCount > 20);
       Assertion.forStep("simple step", fun stats -> stats.RPS > 8);
       Assertion.forStep("simple step", fun stats -> stats.Percent75 >= 102);
       Assertion.forStep("simple step", fun stats -> stats.DataMinKb = 1.0);
       Assertion.forStep("simple step", fun stats -> stats.AllDataMB >= 0.01)]


    let scenario =
        Scenario.create "xunit hello world" [step1]
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 2.0)
        |> Scenario.withAssertions assertions 

    NBomberRunner.registerScenarios [scenario] |> NBomberRunner.runTest
    
