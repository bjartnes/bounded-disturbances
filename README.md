# bounded-disturbances / chaos engineering

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLAs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds.

The bounded-disturbance term is borrowed from control theory and more specifically robust control. It is probably covering excactly the same as chaos engineering, but I prefer it as it deals less with chaos and more with designing a system with a fixed set of parameters (timeouts, retries etc) that perform within a given set of constraints when the system performs different from its normal/design conditions. So rather than talking about chaos, we will talk about how robust the system is when a dependency that normally responds within 10 milliseconds responds in 10 seconds for 1% of its requests.

In the workshop we use https://github.com/App-vNext/Polly for resilience-mechanisms and https://github.com/Polly-Contrib/Simmy as a chaos-monkey to introduce our disturbances. For load testing we use https://github.com/PragmaticFlow/NBomber and XUnit.

This runs on dotnet core in VS Code and should work on Windows/Linux/Mac.

# How to run

# Prerequisites
To run the workshop clone the repo, install dotnet core 3.1 or greater from https://dotnet.microsoft.com/download
and VS Code from https://code.visualstudio.com/

# Starting the API

It should work to use CTRL+SHIFT+P and select "Run Build Task" and then select - "watch" to start the API.
(If it does not work to run watch, then it is possible to run "dotnet watch -p api-under-test/api-under-test.csproj run" in the console and make sure it starts listening on localhost:5001)
The API will reload once changed and should be exposed at https://localhost:5001/weatherforecast

# Run the tests
CTRL+SHIFT+P and "Run Test Task" - "run tests defined in program.fs" should run the loadtest. They will take approximately 10 seconds to run. 

# Challenges

Comment in/out the challenges in load-tests/program.fs to run the individual challenges.
Fill out the correct solution in the corresponding api-under-test/controllers/challengeXcontroller.cs in the GetPolicy method to make sure the test runs without errors.  

![image](https://user-images.githubusercontent.com/1174441/75037092-aa399100-54b3-11ea-85b2-a1511bd42379.png)

## K6 tests

K6 is not really part of the workshop, but it is a load-testing tool that is often used and if you like to try you can run k6 tests locally with ease against the same endpoints. K6 runs inside a docker image, so most of the setup is related to making sure docker works. 

Install docker.

`docker pull loadimpact/k6`

In VS Code
Ctrl-Shift-P -> Run test task -> Run k6test Windows|Linux

Play with load-test config by modifying k6-tests/challenge0.test.js

![K6](https://user-images.githubusercontent.com/1174441/75426378-d1c1ab00-5944-11ea-8cd7-77574fed3c01.png)

