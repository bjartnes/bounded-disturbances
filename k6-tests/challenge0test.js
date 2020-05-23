    // An idea behind this workshop is to do red-green resiliency testing
    // We should ideally introduce a failing test, then repair it with resiliency
    // Ideally we should be able to run our code like this in production - we wouldn't all the time probably, but we could
    // In testing/staging/preproduction environments we surely could run with the Simmy errors all the time, as long as
    // we mitigate the errors with Polly as we introduce them.

    // In the upcoming challenges someone has written the failing load tests and introduced Simmy the chaos monkey
    // It is up to you to write and tune Polly resiliency logic to mitigate the errors and pass the SLAs as defined by the NBomber tests 


import http from "k6/http";
import { Rate } from "k6/metrics";
import { Counter } from "k6/metrics";

export let options = {
  vus       : 10,
  duration  : "30s",
  rps       : 25, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.99'],
    '200 OK count': ['count>200'],
    'http_req_duration': ['p(95)<200']
 }
}

const myOkRate = new Rate("200 OK rate");
const myOkCounter = new Counter("200 OK count");

export default function() {
  let response = http.get("https://host.docker.internal:5001/weatherforecast_challenge0");
  let resOk = response.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
};
