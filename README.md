# Bounded-disturbances / chaos engineering

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLOs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds.

The bounded-disturbance term is borrowed from control theory and more specifically robust control. 
It is probably covering a lot of chaos engineering principles, but only a subset of it.
In its current form it deals less with the entire field of chaos engineering and more with designing a system with a fixed set of parameters (timeouts, retries etc) that perform within a given set of constraints when the system performs different from its normal/design conditions. 

In the workshop we use https://github.com/App-vNext/Polly for resilience-mechanisms and https://github.com/Polly-Contrib/Simmy as a monkey to introduce our disturbances. For load testing we use k6 (https://k6.io/). For visualization we report to InfluxDB and present the data in Grafa. 

This runs on dotnet core in VS Code and should work on Windows/Linux/Mac, but it requires Docker to work properly set up like
described here https://code.visualstudio.com/docs/remote/containers.

For an intro to Polly - which could be a nice background - I would recommend watching  https://www.youtube.com/watch?v=2kfCXMoVCqM and perhaps code-along with the examples here https://github.com/App-vNext/Polly-Samples/

For a background on why we have been working on this in NRK see https://aka.ms/cloudstories-3
# Setting it up
On mac, use <kbd>⌘</kbd> instead of <kbd>ctrl</kbd>.

## Create the codespace
<img width="773" alt="image" src="https://github.com/user-attachments/assets/c74e7d43-72c3-43d3-9a40-018de326fb94">

## Setting up dashboards and datasources

Run the task (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd>) to run tasks and run the following tasks. (If you can not find the task, try to type "build" for examle to find the "Run Build Task", hit enter then search for "run and watch". If prompted by anything just hit "Continue without scanning the task output")
![Continue without scanning the task output](https://user-images.githubusercontent.com/1174441/92082006-40843300-edc4-11ea-893e-64a880408def.png)

1. Run Task: setup logging and dashboards
2. Run Build Task: Run and watch web api
3. Run Test Task: Run k6test (choose Intro for example)

![Creating databases and so on](https://user-images.githubusercontent.com/1174441/92223130-4cddbe00-eea0-11ea-80a9-8aa5c7e2d7d1.png)

# Starting the API
It should work to use <kbd>CTRL</kbd>+<kbd>SHIFT</kbd> + <kbd>P</kbd> and select "Run Build Task" and then select - "watch" to start the API.
 
The API will reload once changed and should be exposed at http://localhost:5555/weatherforecast_intro

The watch and test commands runs in different terminal tabs, see the red ring in the bottom picture of K6 in this README to see how to select the tab if you can not see anything happenning as you run tasks.

# Run the tests
<kbd>CTRL</kbd>+<kbd>SHIFT</kbd>+<kbd>P</kbd> and "Run Test Task" - "k6 test", then select the number of the test you want to run. This should run the loadtest. They will take approximately 10 seconds to run. 

# Dashboards
The dashboards at http://localhost:3000 (admin/admin if you are - you should not be - prompted by anything. Should not require login.).

The dashboards can be tricky to find
![Opening dashboards in Grafana](https://user-images.githubusercontent.com/1174441/100731846-37f3c680-33cc-11eb-9f1d-9c3a91054f91.png)

# Challenges 
Open the files such as challenge1test.js and read the instructions in the top of the file. 
There is a corresponding Controller in api-under-test/Controllers/Challenge1Controller.cs where you must do some
modifications to get the loadtest to pass.

In VS Code
<kbd>CTRL</kbd>+<kbd>SHIFT</kbd>+<kbd>P</kbd> > Run test task -> Run k6test Windows|Linux

## Intro Challenge
The intro is just to see that things run properly, so that you don't have to waste time on mechanics of the workshop when actually trying to learn something new, get to now to navigate Grafana etc.
You can access the dashboard called IntroChallenge in Grafana.
There is no Polly, no Simmy here. Try to change the Task.Delay and see if you can see any changes in the dashboard (Remember, if you save and run the dashboard while the test is running, there will be quite a few seconds when everything will fail as the API shuts down and restarts.) You can also add some exceptions.. And play with the setting for options.rps in challengeintrotest.js to see how it changes things.  

## Challenge 1 - Basic retry
We introduce 15% socket errors using Simmy. Try changing the number of retries we perform and see how it affects  
See if you can make the test green. Pay attention to the rate of 200 OK graph. 
Can you do the math by paper? Can you reach 100%? Does failures in this case change the performance of the API?

Can you fullfill the requirement:
- **Given** that 15% of a method internal in the API fails with socket exception
- **When** we load test the API with 250 rps for 10 secondss
- **Then**: we expect
  - the failure rate of the API to be less than 99%
  - the latency of the 95th percentile to be less than 200 ms

## Challenge 2 - Timeouts
In this challenge we introduce latency of 1 second in 10% of the requests.  
We have a requirement to be faster than than 200 ms in the 95th percentile.
The correctness requirements are not so hard to meet.

Try to practice formulating a given/when/then structure as above.

## Challenge 3 - Timeouts, and not giving up
Now, let us see if we can timeout, but not give up. Maybe, if things are actually just flaky we can try again.
Remember to compose the policies in the right order, the rightmost argument is the one that happens first (if you draw
policies as a block diagram, the rightmost policy will be the inner loop of your logic.)

Try to practice formulating a given/when/then structure.

## Challenge 4 - Timeouts and errors
Now we are getting closer to real life scenarios. We can have both exceptions and slow responses. 
Simmy can simulate both these. In this case, we require quite high correctness, even if our service could be failing quite a lot.  

Try to practice formulating a given/when/then structure.

What happens if you increase the latency of GetForecasts? How robust is our new strategy to changes in the system? What drove it to this?

## Challenge 5 
In .NET when we do timeouts we do that through cancellation. There are a few ways for a framework to start and stop something that should be cancelled. What we call optimistic timeout relies on using co-operative timeout, which means passing a cancellationtoken around. If that cancellationtoken requests a cancellation, then tasks should abort what they are doing. The alternative, pessimistic timeout, is not something we will dig into in this workshop but you can read about it here https://github.com/App-vNext/Polly/wiki/Timeout#pessimistic-timeout.

In order to pass a cancellationtoken, you will need to either create a new one or use one that you already have. In ASP.NET MVC, if you add a CancellationToken parameter to the action signature the framework will automatically bind HttpContext.RequestAborted to it. That means if a connection is closed etc, the request to cancel operations is passed on. We will look at that in a later challenge. In Polly, you can use the CancellationToken.None when calling ExecuteAsync to get a new cancellationtoken and pass that on. Using CancellationToken.None is enough for this challenge.

In the challenge we want to cancel all requests so that they do not take longer than 100 ms, even though that means failing some more. What real-life reasons could you think of where this makes sense? What could be the downside of cancelling requests that take 
a little longer? We need to create a propagate a CancellationToken so that Simmy has a way of cancelling and cleaning up tasks and making sure the request is actually aborted.

## Challenge 6 
This is really very much the same as challenge 5, but try to use the CancellationToken from the controller instead of creating a new one. You need to properly send it through the ExecuteAsync method in order for Polly to cancel things properly when it times out.
The behavior for this particular test is very much the same, but can you think of cases where the two ways of doing it behaves differently?

## Challenge 7 
Idempotency and retrying... 
Hint: Look for what the System.InvalidOperationException says.
There is no dashboard for this challenge.

## Challenge 8 - Thinking fast and slow
Some resources are slower than other. We want fast retries to make the fast requests fast when things are slow, but we also
want the slowest requests to succeed. 


## Challenge 11
By setting a timeout of 10 ms in the k6 test, the requests will be cancelled by the client.
The Challenge 11 controller is set up as a timebomb, and 5% of the requests will take longer and if they are not cancelled they will exit the web api application and crash the entire service. Therefore, the challenge requires that the API is written in a way such that it will cancel the request and its tasks when the client closes the connection.

Try to think of how this timing out based on closed connections can affect an API with respect to issues such as caching slow requests.

# Comments

There are several things worth mentioning that one should look into that is ignored in this workshop to make it easy to work with the code. 
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi/blob/master/SimmyDemo_WebApi/Startup.cs#L70

# Troubleshooting and other comments on docker setup
If you struggle with docker locally, please try Visual Studio Codespaces.

## Port errors
If any of the ports are in use on your system things will not start up properly. 
On windows to see who for example binds to port 3000 try

```
netstat -aof | findstr :3000
```
It takes a while sometimes to scan, but killing the process and try to reopen the folder should work.


## "docker-credential-gcloud not installed or not available in PATH"
Delete the relevant references in <user-mappe>\.docker\config.json


## Build errors
I have seen the following error, and I think it is when you build locally and then afterwards in the images, and the bin and obj folders
contains messed up references. Try to delete api-under-test/bin and api-under-test/obj if you see this error.
```
(/usr/share/dotnet/sdk/3.1.300/Sdks/Microsoft.NET.Sdk/targets/Microsoft.PackageDependencyResolution.targets(234,5): error MSB4018: NuGet.Packaging.Core.PackagingException: Unable to find fallback package folder 'C:\Program Files\dotnet\sdk\NuGetFallbackFolder'. [/workspace/api-under-test/api-under-test.csproj])
```

## If you want to have Docker metrics in Grafana turn on this
![You do not have to, but for Docker metrics](https://user-images.githubusercontent.com/1174441/82738633-5c922600-9d39-11ea-83bb-ea0bf358645a.png)

You also have to uncomment the line here:  
https://github.com/bjartwolf/bounded-disturbances/blob/1adb836b14f2947b19360f06a4b7c39084ec4c4e/telegraf-volume/telegraf.conf#L171

I haven't tested it in a while, but it should allow for scraping of docker metrics to show in Grafana.
