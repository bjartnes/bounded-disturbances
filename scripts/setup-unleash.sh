curl -c kaker 'http://172.17.0.1:4242/auth/simple/login' \
  -H 'Content-Type: application/json' \
  --data-raw '{"username":"admin","password":"unleash4all"}'

export UNLEASHTOKEN=$(curl -b kaker 'http://172.17.0.1:4242/api/admin/api-tokens' \
  -H 'Content-Type: application/json' \
  --data-raw '{"username":"workshop","type":"CLIENT"}' -s \
  | jq ".secret")

dotnet user-secrets init --project api-under-test/api-under-test.csproj
dotnet user-secrets set "UnleashToken" $UNLEASHTOKEN --project api-under-test/api-under-test.csproj