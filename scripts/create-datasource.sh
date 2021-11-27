#!/usr/bin/env bash
set -o errexit
set -o pipefail
set -o nounset
# TODO MIGHT NEED TO ADD DATABASE REPORTDB IN THE LATEST VERSION
curl 'http://172.17.0.1:3000/api/datasources' \
  -H 'accept: application/json, text/plain, */*' \
  -H 'Referer: https://172.17.0.1:3000' \
  -H 'x-grafana-org-id: 1' \
   -H 'content-type: application/json' \
  --data-binary '{"name":"InfluxDB","type":"influxdb","access":"proxy","isDefault":true}' \
  --compressed

curl 'http://172.17.0.1:3000/api/datasources/1' \
  -X 'PUT' \
  -H 'accept: application/json, text/plain, */*' \
  -H 'Referer: http://172.17.0.1:3000/datasources/edit/1/' \
  -H 'x-grafana-org-id: 1' \
  -H 'content-type: application/json' \
  --data-binary '{"id":1,"orgId":1,"name":"InfluxDB","type":"influxdb","typeLogoUrl":"","access":"proxy","url":"http://172.17.0.1:8086","password":"","user":"user","database":"reportdb","basicAuth":false,"basicAuthUser":"","basicAuthPassword":"","withCredentials":false,"isDefault":true,"jsonData":{},"secureJsonFields":{},"version":1,"readOnly":false,"secureJsonData":{"password":"user"}}' \
  --compressed