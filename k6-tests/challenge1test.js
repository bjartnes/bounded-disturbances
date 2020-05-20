    // The next challenge introduces latency of 1 second to 10% of dependencies using Simmy.  
    // Notice that the chaos must be wrapped as the last argument so that it runs inside the retries (try changing that...?)
    // Change the policy in Challenge1Controller.cs
 
import http from "k6/http";
import { Rate } from "k6/metrics";
import { Counter } from "k6/metrics";

export let options = {
  vus       : 50,
  duration  : "6s",
//  rps       : 25, //max requests per second, increase to go faster
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
  let response = http.get("https://host.docker.internal:5001/weatherforecast_challenge1");
  let resOk = response.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
};

