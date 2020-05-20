// To run demos uncomment the tests you want to run, start with the intro 

[<EntryPoint>]
let main args =


    //Tests0.``Challenge 0``()

   //Tests1.``Challenge 1``()

    // Sometimes speed is all we care about
    //Tests2.``Challenge 2``()

    //Tests3.``Challenge 3``()
    //Tests4.``Challenge 4``()

    //Tests5.``Challenge 5``()

    // This needs more refactoring..
    //Tests6.``Challenge 6``()




    // Challenge 10 requires Docker and is running k6 to close ports
    // It is a bit hairy, but you should see
    //  the api throwing socketexceptions and if you run >netstat -a -n from a cmd.exe on windows in admin mode you should se a lot of time-wait sockets that are open.
    // I havent yet been able to make a very sweet demo fo this with red green tests, but I guess I will... Somehow.
    // run the test task called k6 test 10 ...
    // You need to pay attention to the api output, it should start spitting out errors. netstat -a -n should show a lot of stuff
    0