    // If you look carefully this will struggle - it is not idempotent... And injected exceptions will not trigger the underlying method
import http from "k6/http";
import { Rate } from "k6/metrics";
import { Counter } from "k6/metrics";

export let options = {
  vus       : 5,
  duration  : "4s",
  rps       : 200, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.99'],
    '200 OK count': ['count>200'],
    'http_req_duration': ['p(95)<100']
 }
}

const myOkRate = new Rate("200 OK rate");
const myOkCounter = new Counter("200 OK count");

export default function() {
  let response = http.get("http://localhost:5000/weatherforecast_challenge7");
  let resOk = response.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
};

