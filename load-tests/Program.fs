// To run demos uncomment the tests you want to run, start with the intro 

[<EntryPoint>]
let main args =

    // Pure load-testing, no polly, no simmy, just a simple run to see how the workshop is structured and make sure things run
    // Start the API with running the build task called watch (it will start the API and reload it on changes)
    // Then run the test task called "run test tasks in program.fs (this file...)"
    IntroTests.Intro()

    // We always add errors and fixes at the same time. Or try to :)
    // In these examples we typically have defined performance requirements
    // and a defined chaos and 

    // Some motivation and intro to framework

    // Tests0.``Challenge 0``()
    // Tests1.``Challenge 1``()
    // Tests2.``Challenge 2``()


    // Some annoying real-world things we must think of regarding cancellations
    // Ideally they could be run first, but this is more of a nescessity than the core 
    // motivation for the workshop, so we add them later (but do NOT skip these)

    0