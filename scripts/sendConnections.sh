while true; do
	curl -i -XPOST 'http://172.17.0.1:8086/write?db=reportdb' --data-binary "timewait value=`ss -s | grep 'timewait' | sed 's/.*timewait//' | sed 's/)//' | xargs echo -n`"
	sleep 1
done