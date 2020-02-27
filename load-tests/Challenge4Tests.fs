module Tests4

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
let ``Challenge 4`` () =
    let step1 = HttpStep.create ("simple step", (fun (_sc: StepContext<unit> ) ->  Http.createRequest "GET" "https://localhost:5001/weatherforecast_challenge4")) 

    let assertions = [
       Assertion.forStep("simple step", (fun stats -> stats.OkCount > 200), "Should work for the most part");
       Assertion.forStep("simple step", (fun stats -> stats.Percent95 <= 100), "95 percentile should not be much slower than the timeout")]

    let scenario =
        Scenario.create "Challenge 4" [step1]
        |> Scenario.withConcurrentCopies 5 
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 4.0)
        |> Scenario.withAssertions assertions 


    NBomberRunner.registerScenarios [scenario] |> NBomberRunner.runTest