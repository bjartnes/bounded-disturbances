    // Pure load-testing, no polly, no simmy, just a simple run to see how the workshop is structured and make sure things run
    // Start the API with running the build task called watch (it will start the API and reload it on changes)
    // Try to read and look at the stats... If you want to you can play with parameters such as the delay in the controller, length of test-run,
    // concurrency etc and see how it affects the max RPS (requests pr second) 
 
import http from "k6/http";
import { check } from "k6";

export let options = {
  vus       : 50,
  duration  : "1m",
  rps       : 500, //max requests per second, increase to go faster
  discardResponseBodies: true,
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.99'],
    'http_req_duration': ['p(95)<1000'],
    "Errors": ["rate<0.01"],
    "Content OK": ["count>200"]
  }
}

export default function() {
    
  let response = http.get("http://localhost:5000/weatherforecast_intro");

  check( response, { "200 OK": res => res.status === 200 } );

};
