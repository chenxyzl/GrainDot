#!/bin/sh
launch()
{
	./Boot $1 -nodump true -console true > /dev/null 2>&1 &
}

# launch "lobby -id 1"
launch "-t=Login -i=0"
launch "-t=World -i=1"
launch "-t=Home -i=2"
