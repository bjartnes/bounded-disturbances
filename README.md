# bounded-disturbances

This repository is for a workshop to understand how to introduce and deal with a bounded disturbance.
By bounded disturbance I mean an error in a dependency such as less than 1% socket errors and/or less than 10% timeouts of maximum 5 seconds.
Under these bounded disturbances on our dependencies we still want to maintain stricter SLAs to our consumers, such as less than 0.01% errors 
and a 99% response time of less than 150 milliseconds. 


