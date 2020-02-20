# bounded-disturbances

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLAs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds. 

In the workshop we use https://github.com/App-vNext/Polly for resilience-mechanisms and https://github.com/Polly-Contrib/Simmy as a chaos-monkey to introduce our disturbances. For load testing we use https://github.com/PragmaticFlow/NBomber

This runs on dotnet core in VS Code and should work on Windows/Linux/Mac.

To run the workshop clone the repo, install dotnet core 3.1 or greater from https://dotnet.microsoft.com/download

It should work to use CTRL+SHIFT+P and select Run Build Task - watch to start the API.
The API will reload once changed and should be exposed at https://localhost:5001/weatherforecast

CTRL+SHIFT+P and Run Test Task - run loadtest should run the loadtest. They will take approximately 10 seconds to run. 


# Challenges

I will add different challenges here...

![Workshop example](https://user-images.githubusercontent.com/1174441/74938562-88c2a180-53ee-11ea-959c-7c8383fdf315.png)
