#!/bin/sh

# Import secrets as environment variables
SCRIPT_DIR=$( dirname -- "$( readlink -f -- "$0"; )"; )
. $SCRIPT_DIR/dockerSecrets.sh

echo "Using MongoDB connection string:"
echo $ZuneNetContext__ConnectionString

docker compose down -v
docker compose build --no-cache
docker compose up -d --remove-orphans
docker compose logs --follow --tail 0
