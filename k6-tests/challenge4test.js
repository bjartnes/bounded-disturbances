// This scenario show how to test for both latency and exceptions.
import http from "k6/http";
import { Rate, Trend, Counter } from "k6/metrics";

export let options = {
  vus       : 10,
  duration  : "4s",
  rps       : 200, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.999'],
    '200 OK count': ['count>200'],
    'http_req_duration': ['p(95)<200']
 }
}

export let TrendRTT = new Trend("RTT");
const myOkRate = new Rate("200 OK rate");
const myOkCounter = new Counter("200 OK count");

export default function() {
  let response = http.get("http://localhost:5555/weatherforecast_challenge4");
  let resOk = response.status === 200;
  TrendRTT.add(response.timings.duration);
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
};

