wget https://golang.org/dl/go1.16.5.linux-amd64.tar.gz
rm -rf /usr/local/go && tar -C /usr/local -xzf go1.16.5.linux-amd64.tar.gz
# Blah, export kjører bare i scriptet og så er PATH ikke satt. Dustebash.

export PATH=$PATH:/usr/local/go/bin
go version

go install github.com/k6io/xk6/cmd/xk6@latest
~/go/bin/xk6 build --with github.com/k6io/xk6-distributed-tracing@latest
export PATH=$PATH:~/go/bin
