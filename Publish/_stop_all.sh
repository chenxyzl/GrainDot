#!/bin/sh

ps aux|grep Boot|grep -v grep > ./svc_pid_file.txt


# killall -SIGUSR1 Boot
killall Boot

set i=0
while test $(ps aux|grep Boot|grep -v grep|wc -l) -ne 0
do
	sleep 1
	let "i=i+1"
    echo "-------等待结束:$i-------"
done

if test $(ps aux|grep Boot|grep -v grep|wc -l) -eq 0
then
	echo "Kill All Success!"
else
	echo "Kill All Fail!"
fi
echo $(date) " run " $0>> record