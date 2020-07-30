
    // Some annoying real-world things we must think of regarding cancellations
    // Ideally they could be run first, but this is more of a nescessity than the core 
    // motivation for the workshop, so we add them here, sort of in the middle of the workshop.
    // By now you should be motivated, but not so tired that you skip the hard parts.

import http from "k6/http";
import { Rate } from "k6/metrics";
import { Counter } from "k6/metrics";

export let options = {
  vus       : 10,
  duration  : "4s",
  rps       : 200, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.5'],
    '200 OK count': ['count>200'],
    'http_req_duration': ['p(95)<100']
 }
}

const myOkRate = new Rate("200 OK rate");
const myOkCounter = new Counter("200 OK count");

export default function() {
  let response = http.get("http://localhost:5000/weatherforecast_challenge4");
  let resOk = response.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
};

