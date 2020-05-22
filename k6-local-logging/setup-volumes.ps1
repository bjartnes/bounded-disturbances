echo "Setting up volumes"

docker network create monitoring
docker volume create grafana-volume
docker volume create influxdb-volume
docker volume create telegraf-volume #--opt type=none --opt device=/telegraf-volume --opt o=bind

echo "Start Incluxdb and Grafana with 'docker-compose up'"
echo "Refer to 'docker-compose.yml' info about ports. Login credentials for Grafana is 'admin'/'admin''"