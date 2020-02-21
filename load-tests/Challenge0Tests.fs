module Tests0

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
let ``Challenge 0`` () =

    let step1 = HttpStep.create ("step1", (fun (_sc: StepContext<unit> ) -> Http.createRequest "GET" "https://localhost:5001/weatherforecast_challenge0"))

    let assertions = [
       Assertion.forStep("step1", (fun stats -> stats.OkCount > 200), "Need 200 requests to have some data");
       Assertion.forStep("step1", (fun stats -> (float stats.FailCount) / (float stats.OkCount) <= 0.0001), "Max 0.01% errors");
       Assertion.forStep("step1", (fun stats -> stats.Percent95 <= 200), "95 percentile should be fast")]

    let scenario =
        Scenario.create "Challenge 0" [step1]
        |> Scenario.withConcurrentCopies 50
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 4.0)
        |> Scenario.withAssertions assertions 

    NBomberRunner.registerScenarios [scenario] |> NBomberRunner.runTest
    
