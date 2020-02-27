module Tests3

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
let ``Challenge 3`` () =
    let step1 = HttpStep.create ("simple step", (fun (_sc: StepContext<unit> ) -> Http.createRequest "GET" "https://localhost:5001/weatherforecast_challenge3"))

    let assertions = [
       Assertion.forStep("simple step", (fun stats -> stats.OkCount > 200), "Need 200 requests to have some data");
       Assertion.forStep("simple step", (fun stats -> stats.FailCount < 5), "Max five errors");
       Assertion.forStep("simple step", (fun stats -> stats.Percent95 <= 200), "95 percentile should be fast")]

    let scenario =
        Scenario.create "Challenge 3" [step1]
        |> Scenario.withConcurrentCopies 50
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 4.0)
        |> Scenario.withAssertions assertions 

    NBomberRunner.registerScenarios [scenario] |> NBomberRunner.runTest