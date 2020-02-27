// To run demos uncomment the tests you want to run, start with the intro 

[<EntryPoint>]
let main args =

    // Pure load-testing, no polly, no simmy, just a simple run to see how the workshop is structured and make sure things run
    // Start the API with running the build task called watch (it will start the API and reload it on changes)
    // Then run the test task called "run test tasks in program.fs (this file...)"
    // Try to read and look at the stats... If you want to you can play with parameters such as the delay in the controller, length of test-run,
    // concurrency etc and see how it affects the max RPS (requests pr second) 
    //IntroTests.Intro()

    // An idea behind this workshop is to do red-green resiliency testing
    // We should ideally introduce a failing test, then repair it with resiliency
    // Ideally we should be able to run our code like this in production - we wouldn't all the time probably, but we could
    // In testing/staging/preproduction environments we surely could run with the Simmy errors all the time, as long as
    // we mitigate the errors with Polly as we introduce them.

    // In the upcoming challenges someone has written the failing load tests and introduced Simmy the chaos monkey
    // It is up to you to write and tune Polly resiliency logic to mitigate the errors and pass the SLAs as defined by the NBomber tests 

    //Tests0.``Challenge 0``()

    // The next challenge introduces latency of 1 second to 10% of dependencies using Simmy.  
    // Notice that the chaos must be wrapped as the last argument so that it runs inside the retries (try changing that...?)
    // Change the policy in Challenge1Controller.cs
    //Tests1.``Challenge 1``()

    // Sometimes speed is all we care about
    //Tests2.``Challenge 2``()

    // This scenario show how to test for both latency and exceptions.
    //Tests3.``Challenge 3``()

    // Some annoying real-world things we must think of regarding cancellations
    // Ideally they could be run first, but this is more of a nescessity than the core 
    // motivation for the workshop, so we add them here, sort of in the middle of the workshop.
    // By now you should be motivated, but not so tired that you skip the hard parts.

    Tests4.``Challenge 4``()
    0