module Tests2

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
let ``Challenge 2`` () =
    let step1 = HttpStep.create ("simple step", (fun (_sc: StepContext<unit> ) -> Http.createRequest "GET" "https://localhost:5001/weatherforecast_challenge2"))

    let assertions = [
       Assertion.forStep("simple step", (fun stats -> stats.OkCount > 200), "Need 200 requests to have some data");
       Assertion.forStep("simple step", (fun stats -> stats.FailCount < 500), "We can have a lot of errors");
       Assertion.forStep("simple step", (fun stats -> stats.Percent95 <= 40), "95 percentile should be very fast")]

    let scenario =
        Scenario.create "Challenge 2" [step1]
        |> Scenario.withConcurrentCopies 10
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 4.0)
        |> Scenario.withAssertions assertions 

    NBomberRunner.registerScenarios [scenario] |> NBomberRunner.runTest