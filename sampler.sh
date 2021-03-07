function sample(){    
    docker inspect -f '{{.NetworkSettings.IPAddress }}' $(docker ps -q) | awk -vhostname="$(hostname)" 'NF {printf "$:%s:%s\n", hostname, $1}'
    docker ps --format "{{.Ports}}" | awk -vORS=' or '   -F [:-] 'IF $2 {print $2}' | sed '$s/...$//' | xargs -0 -I {} bash -c "timeout 5 sudo tcpdump tcp -nn -c 100 -i any port not 50777 '({})'" | awk '{gsub(/:/,"");print "#"$3"$"$5} '
}

host=$(docker inspect -f '{{.Config.Env}}' $(docker ps --filter publish=50777 -q) |  grep -oP '(?<=NOMAD_HOST_IP_tcp=).*?(?= )')
(while true; do sample ; sleep 3; done) | (while true; do nc "${host}" 50777; done)
