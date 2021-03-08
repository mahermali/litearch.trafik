#!/bin/bash
declare host="0.0.0.0"

while true; do
      printf "Start ..\n"
      host=$(docker inspect -f '{{.Config.Env}}' $(docker ps --filter publish=50777 -q) |  grep -oP '(?<=NOMAD_HOST_IP_tcp=).*?(?= )')
      printf "Host: $s" "$host"
      docker inspect -f '{{.NetworkSettings.IPAddress }}' $(docker ps -q) | awk -vhostname="$(hostname)" 'NF {printf "$:%s:%s\n", hostname, $1}' > /dev/tcp/"${host}"/50777
      docker ps --format "{{.Ports}}" | awk -vORS=' or ' -F [:-] 'IF $2 {print $2}' | sed '$s/...$//' | xargs -0 -I {} bash -c "timeout 3 tcpdump tcp -nn -c 100 -i any port '({} and not 50777)'" | awk '{gsub(/:/,"");print "#"$3"$"$5} ' > /dev/tcp/"${host}"/50777
      printf "Iteration done\n"
      sleep 3
done