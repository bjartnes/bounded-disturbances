    // Pure load-testing, no polly, no simmy, just a simple run to see how the workshop is structured and make sure things run
    // Start the API with running the build task called watch (it will start the API and reload it on changes)
    // Try to read and look at the stats... If you want to you can play with parameters such as the delay in the controller, length of test-run,
    // concurrency etc and see how it affects the max RPS (requests pr second) 
 
import http from "k6/http";
import { check } from "k6";
import { Trend, Rate } from "k6/metrics";

export let options = {
  vus       : 50,
  duration  : "1m",
  rps       : 500, //max requests per second, increase to go faster
  discardResponseBodies: true,
  insecureSkipTLSVerify : true //ignore that localhost cert doesn't match host.docker.internal
}

export let TrendRTT = new Trend("RTT");

let params = {
  headers: {
    "Accept": "*/*",
    "Accept-Encoding": "gzip",
    "User-Agent" : "nrktv-k6-loadtest"
  },thresholds: {
    '200 OK rate': ['rate>0.99'],
    "RTT": [
      "p(95)<200"
    ],
    "Errors": ["rate<0.01"],
    "Content OK": ["count>200"]
 }
}

const myOkRate = new Rate("200 OK rate");

export default function() {
    
  let response = http.get("http://localhost:5000/weatherforecast_intro", params);

  let resOk = response.status === 200;
  myOkRate.add(resOk);
 
  check( response, { "200 OK": res => res.status === 200 } );

  TrendRTT.add(response.timings.duration);
  
};
