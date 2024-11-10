import http from "k6/http";
import { Trend, Rate, Counter } from "k6/metrics";

export let TrendRTT = new Trend("RTT");
const myOkRate = new Rate("200 OK rate");
const myOkCounter = new Counter("200 OK count");

export let options = {
  vus       : 10,
  duration  : "10s",
  rps       : 200, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '200 OK rate': ['rate>0.8'],
    '200 OK count': ['count>200'],
    'http_req_duration': ['p(95)<100']
 }
}

let params = {
  headers: {
    "Accept": "*/*",
    "Accept-Encoding": "gzip",
    "User-Agent" : "nrktv-k6-loadtest"
  },
  timeout: 50 //ms
}

export default function() {
    
  let res = http.get("http://localhost:5000/weatherforecast_challenge11", params);
  TrendRTT.add(res.timings.duration);
  let resOk = res.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk);
  
};