#!/usr/bin/env bash
set -o errexit
set -o pipefail
set -o nounset
__dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
__root="$(cd "$(dirname "${__dir}")" && pwd)"
API_KEY=$(curl -s -X POST -H "Content-Type: application/json" -d '{"name":"curl", "role": "Admin"}' http://admin:admin@grafana:3000/api/auth/keys | tee .api-key.json | jq -r .key)
curl -v -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $API_KEY" \
    -d @"$__root/k6-dashboards/'Challenge 0-Dashboard.json'" \
    http://grafana:3000/api/dashboards/db