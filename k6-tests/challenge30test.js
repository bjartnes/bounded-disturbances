import http from "k6/http";
import { Trend, Rate, Counter } from "k6/metrics";

export let TrendRTT = new Trend("RTT");
const myOkRate = new Rate("_200_OK_rate");
const myOkCounter = new Counter("_200_OK_count");

export let options = {
  vus       : 10,
  duration  : "10s",
  rps       : 100000, //max requests per second, increase to go faster
  insecureSkipTLSVerify : true, //ignore that localhost cert doesn't match host.docker.internal
  thresholds: {
    '_200_OK_rate': ['rate>0.99'],
    '_200_OK_count': ['count>2000'],
    'http_req_duration': ['p(99)<1']
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
    
  //https://www.nginx.com/blog/nginx-caching-guide/
  let res = http.get("http://localhost:80/weatherforecast_challenge3", params);
  TrendRTT.add(res.timings.duration);
  let resOk = res.status === 200;
  myOkRate.add(resOk);
  myOkCounter.add(resOk); 
  
};


// Cheat code
  // mkdir /cache
  // chmod 777 /cache
  // code /etc/nginx/sites-enabled
  // add proxy_cache_path /cache levels=1:2 keys_zone=my_cache:10m max_size=10g inactive=60m use_temp_path=off;
  // modify     location / { proxy_cache my_cache; proxy_pass http://172.17.0.1:5555; 
  // Response.Headers.Add("Cache-Control", "public, max-age=500");
  // need to create /cache dir and chmod it to read and write
  // files are in /etc/nginx/sites-enabled
 // sudo service start nginx
  // see how many errors we can not-handle and still make this run...
