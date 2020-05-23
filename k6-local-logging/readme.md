# Lokaltoppsett av InfluxDB and Grafana

## Prerequisites
- Docker
- (Powershell core)

Du kan også kjøre innholdet i ps1-filene manuelt

## Sette opp volumes
Kjør `pwsh setup-volumes.ps1`

## Start InfluxDB and Grafana
Kjør `docker-compose up` (Du kan evt også legge på `-d` for å kjøre kontainere i bakgrunnen)

## Lag database for test repportering
Kjør `pswh create-report-db.ps1`

ms-vscode-remote.remote-containers

## Grafana
Åpne http://localhost:3000
Både brukernavn og passord er `admin`

### Initielt oppsett
Følg flyten og sett opp datakilde ++

Nyttige parametre:
Influx
* url: http://influxdb:8086
* database: reportdb
* user: "user"
* passord: "user"

### Sett opp Dashboard
importer `k6-load-testing-results_rev3.json`

## k6 Logging til InfluxDB
Spørs kanskje litt hva som funker på windows, linux og mac her...
docker.internal.host funker på Windows, kanskje på mac...?
Kjør `k6 run --out influxdb=http://localhost:8086/reportdb`
Kjør `k6 run --out influxdb=http://docker.interal.host:8086/reportdb`
