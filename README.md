# bounded-disturbances / chaos engineering

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLAs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds.

The bounded-disturbance term is borrowed from control theory and more specifically robust control. It is probably covering excactly the same as chaos engineering, but I prefer it as it deals less with chaos and more with designing a system with a fixed set of parameters (timeouts, retries etc) that perform within a given set of constraints when the system performs different from its normal/design conditions. So rather than talking about chaos, we will talk about how robust the system is when a dependency that normally responds within 10 milliseconds responds in 10 seconds for 1% of its requests.

In the workshop we use https://github.com/App-vNext/Polly for resilience-mechanisms and https://github.com/Polly-Contrib/Simmy as a chaos-monkey to introduce our disturbances. For load testing we use k6 (https://k6.io/). For visualization we report to InfluxDB and present the data in Grafa. 

  This runs on dotnet core in VS Code and should work on Windows/Linux/Mac, but it requires Docker to work properly set up like
  described here https://code.visualstudio.com/docs/remote/containers.

  It also runs online at https://online.visualstudio.com/ which is the easiest way to set things up. 
  # How to run


  # In Visual Studio Codespace
  Go to https://online.visualstudio.com/, create an account etc. The first month is free, but you need a credit card. If you have used your month, you must pay, but it is cheap if you just clean up your resources.

  Create a new codespace, point it to this repo and start. I just just the default machine size.

  Once logged in and everything is set up (takes a few minutes) try to
  Ctrl+Shift+P the following tasks.

  * Run Build Task: Run and watch web api
  * Run Task: setup logging and dashboards
  * Run Test Task: Run k6test (choose Intro for example)

  To see this in grafana in your browser, you need a few more tricks.
  Run task: forward grafana port
  Codespaces: Copy port URL (select the 3000 one)

  Paste that long, cryptic url in your browser and you should be able to navigate to the
  Challenge 0 dashboard (I havent made the other ones yet.)


# Locally with docker and VS Code 
To run the workshop clone the repo, install VS Code from https://code.visualstudio.com/

Install docker. 

You will get a lot of these ![install  extension](https://user-images.githubusercontent.com/1174441/82751431-85590080-9db7-11ea-8a6a-7728a0a1c877.png) 
to install extensions, and then asked to relead the folder in the Docker images and just go yes, yes, sure, reload etc. And
wait when it asks you too, downloading and building all the images can take a while the first time.

If it does not open it automatically
![Open the folder in the workspace](https://user-images.githubusercontent.com/1174441/82751510-04e6cf80-9db8-11ea-9040-47e122c98e11.png)
## On Windows
Share the repo in Docker
![Turn on filesharing](https://user-images.githubusercontent.com/1174441/82738627-4c7a4680-9d39-11ea-9b6a-ab42b9accec3.png)

I have seen the following error, and I think it is when you build locally and then afterwards in the images, and the bin and obj folders
contains messed up references. Try to delete api-under-test/bin and api-under-test/obj if you see this error.
```
(/usr/share/dotnet/sdk/3.1.300/Sdks/Microsoft.NET.Sdk/targets/Microsoft.PackageDependencyResolution.targets(234,5): error MSB4018: NuGet.Packaging.Core.PackagingException: Unable to find fallback package folder 'C:\Program Files\dotnet\sdk\NuGetFallbackFolder'. [/workspace/api-under-test/api-under-test.csproj])
```
If you want to have Docker metrics in Grafana turn on this
![You do not have to, but for Docker metrics](https://user-images.githubusercontent.com/1174441/82738633-5c922600-9d39-11ea-83bb-ea0bf358645a.png)

Run the scripts in /k6-local-logging/ (setup-volumes.ps1), docker-compose up and create-report-db.ps1. I will write some more details on this, probably wrap it in one script and also make something for Linux Mac on this (it is almost the same commands...)


# Setting up dashboards and datasources

Run the task (Ctrl+Shift+P) to create the database.
![Create the database in Influx](https://user-images.githubusercontent.com/1174441/82750736-660ba480-9db2-11ea-9358-baa1e80d9c63.png)

![Add influx DB datasource](https://user-images.githubusercontent.com/1174441/82738661-9531ff80-9d39-11ea-83a1-690f7c78be6c.png)

# Starting the API

It should work to use CTRL+SHIFT+P and select "Run Build Task" and then select - "watch" to start the API.
Also, the button - I will make screenshots - should work.

Or you can use the button
![To start the API with the play button](https://user-images.githubusercontent.com/1174441/82750720-4c6a5d00-9db2-11ea-8149-90c020fa2148.png)

The API will reload once changed and should be exposed at http://localhost:5000/weatherforecast_intro

The watch and test commands runs in different terminal tabs, see the red ring in the bottom picture of K6 in this README to see how to select the tab if you can not see anything happenning as you run tasks.

# Run the tests
CTRL+SHIFT+P and "Run Test Task" - "k6 test", then select the number of the test you want to run. This should run the loadtest. They will take approximately 10 seconds to run. 

# Dashboards
Looking at the dashboards at http://localhost:3000 (admin/admin) and importing them... will describe more of that too...

# Challenges 
Open the files such as challenge0test.js and read the instructions in the top of the file. 
There is a corresponding Controller in api-under-test/Controllers/Challenge0Controller.cs where you must do some
modifications to get the loadtest to pass.

In VS Code
Ctrl-Shift-P -> Run test task -> Run k6test Windows|Linux

## Intro Challenge
The intro is just to see that things run properly, so that you don't have to waste time on mechanics of the workshop when actually trying to learn something new, get to now to navigate Grafana etc.
You can access the dashboard called IntroChallenge in Grafana.
There is no Polly, no Simmy here. Try to change the Task.Delay and see if you can see any changes in the dashboard (Remember, if you save and run the dashboard while the test is running, there will be quite a few seconds when everything will fail as the API shuts down and restarts.) You can also add some exceptions.. And play with the setting for options.rps in challengeintrotest.js to see how it changes things.  

## Challenge 0 - Basic retry
We introduce 15% socket errors using Simmy. Try changing the number of retries we perform and see how it affects  
See if you can make the test green. Pay attention to the rate of 200 OK graph. 
Can you do the math by paper? Can you reach 100%? Does failures in this case change the performance of the API?

## Challenge 1 - Timeouts
In this challenge we introduce latency of 1 second in 10% of the requests.  
We have a requirement to be faster than than 200 ms in the 95th percentile.
The correctness requirements are not so hard to meet.

## Challenge 2 - Timeouts, and not giving up
Now, let us see if we can timeout, but not give up. Maybe, if things are actually just flaky we can try again.
Remember to compose the policies in the right order, the rightmost argument is the one that happens first (if you draw
policies as a block diagram, the rightmost policy will be the inner loop of your logic.)

# Comments

There are several things worth mentioning that one should look into that is ignored in this workshop to make it easy to work with the code. 
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi/blob/master/SimmyDemo_WebApi/Startup.cs#L70