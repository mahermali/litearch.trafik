#!/bin/bash
declare host="0.0.0.0"

# $Host_IP:Docker_IP:Nomad_Task_Name
# #Source.Port:Target.Port -> Source | Target = Docker IP => Host_IP/Nomad_Task_Name
while true; do
      printf "Start ..\n"
      host=$(docker inspect -f '{{.Config.Env}}' $(docker ps --filter publish=50777 -q) |  grep -oP '(?<=NOMAD_HOST_IP_tcp=).*?(?= )')
      printf "Host: $s" "$host"
      { docker inspect $(docker ps -q) | jq ' .[] | {task: .Config.Env | map(select (.|contains("NOMAD_TASK_NAME"))) [0], ip: .NetworkSettings.IPAddress}' | jq -r --arg host "$host" '"$\($host):\(.ip):\(.task)"' ; docker ps --format "{{.Ports}}" | awk -vORS=' or ' -F [:-] 'IF $2 {print $2}' | sed '$s/...$//' | xargs -0 -I {} bash -c "timeout 3 tcpdump tcp -nn -c 100 -i any port '({} and not 50777)'" | awk '{gsub(/:/,"");print "#"$3":"$5"|"} ' ; } > /dev/tcp/"${host}"/50777
      printf "Iteration done\n"
      sleep 3
done