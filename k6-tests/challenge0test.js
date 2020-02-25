import http from "k6/http";
import { check } from "k6";
import { Trend } from "k6/metrics";

export let options = {
  vus       : 50,
  duration  : "10s",
  rps       : 25, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true //ignore that localhost cert doesn't match host.docker.internal
  
}

export let TrendRTT = new Trend("RTT");

let params = {
  headers: {
    "Accept": "*/*",
    "Accept-Encoding": "gzip",
    "User-Agent" : "nrktv-k6-loadtest"
  },thresholds: {
    "RTT": [
      "p(95)<200"
    ],
    "Errors": ["rate<0.01"],
    "Content OK": ["count>200"]
 }
}

export default function() {
    
  let response = http.get("https://host.docker.internal:5001/weatherforecast_challenge0", params);

  check( response, { "200 OK": res => res.status === 200 } );

  TrendRTT.add(response.timings.duration);
  
};