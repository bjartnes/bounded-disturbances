import http from "k6/http";
import { Rate, Counter, Trend } from "k6/metrics";

export let options = {
  vus       : 10,
  duration  : "4s",
  rps       : 200, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '_200_OK_rate': ['rate>0.99'],
    '_200_OK_count': ['count>200'],
    'http_req_duration': ['p(95)<100']
 }
}

export let TrendRTT = new Trend("RTT");
const myOkRate = new Rate("_200_OK_rate");
const myOkCounter = new Counter("_200_OK_count");

export default function() {
  let response = http.get("http://localhost:5555/weatherforecast_challenge3");
  let resOk = response.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
  TrendRTT.add(response.timings.duration);
};

