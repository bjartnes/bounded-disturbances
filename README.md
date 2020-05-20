# bounded-disturbances / chaos engineering

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLAs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds.

The bounded-disturbance term is borrowed from control theory and more specifically robust control. It is probably covering excactly the same as chaos engineering, but I prefer it as it deals less with chaos and more with designing a system with a fixed set of parameters (timeouts, retries etc) that perform within a given set of constraints when the system performs different from its normal/design conditions. So rather than talking about chaos, we will talk about how robust the system is when a dependency that normally responds within 10 milliseconds responds in 10 seconds for 1% of its requests.

In the workshop we use https://github.com/App-vNext/Polly for resilience-mechanisms and https://github.com/Polly-Contrib/Simmy as a chaos-monkey to introduce our disturbances. For load testing we use k6 (https://k6.io/). For visualization we report to InfluxDB and present the data in Grafa. 

This runs on dotnet core in VS Code and should work on Windows/Linux/Mac, but it requires Docker to work properly.

# How to run

# Prerequisites
To run the workshop clone the repo, install dotnet core 3.1 or greater from https://dotnet.microsoft.com/download
and VS Code from https://code.visualstudio.com/

Install docker.

`docker pull loadimpact/k6`

Run the scripts in /k6-local-logging/ (setup-volumes.ps1), docker-compose up and create-report-db.ps1. I will write some more details on this, probably wrap it in one script and also make something for Linux Mac on this (it is almost the same commands...)

# Starting the API

It should work to use CTRL+SHIFT+P and select "Run Build Task" and then select - "watch" to start the API.
(If it does not work to run watch, then it is possible to run "dotnet watch -p api-under-test/api-under-test.csproj run" in the console and make sure it starts listening on localhost:5001)
The API will reload once changed and should be exposed at https://localhost:5001/weatherforecast_intro

The watch and test commands runs in different terminal tabs, see the red ring in the bottom picture of K6 in this README to see how to select the tab if you can not see anything happenning as you run tasks.

If you struggle with ssl trust run:
`dotnet dev-certs https --trust`

If you are on Ubuntu (maybe also other linux distros), I suggest you rewrite your test to use http at localhost:5000 instead (search and replace, probably) unless you know how to fix it. Or pairprogram with someone.

# Run the tests
CTRL+SHIFT+P and "Run Test Task", then select the number of the test you want to run. This should run the loadtest. They will take approximately 10 seconds to run. 

# Challenges
Open the files such as challenge0test.js and read the instructions in the top of the file. 
There is a corresponding Controller in api-under-test/Controllers/Challenge0Controller.cs where you must do some
modifications to get the loadtest to pass.

In VS Code
Ctrl-Shift-P -> Run test task -> Run k6test Windows|Linux

## Comments

There are several things worth mentioning that one should look into that is ignored in this workshop to make it easy to work with the code. 
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi/blob/master/SimmyDemo_WebApi/Startup.cs#L70