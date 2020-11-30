#!/usr/bin/env bash
set -o errexit
set -o pipefail
set -o nounset

curl -X POST -G http://172.17.0.1:8086/query --data-urlencode 'q=CREATE DATABASE reportdb'

./scripts/create-datasource.sh
./scripts/create-dashboard.sh