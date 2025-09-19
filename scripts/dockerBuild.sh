#!/bin/sh
set -e

# Import secrets as environment variables
SCRIPT_DIR=$( dirname -- "$( readlink -f -- "$0"; )"; )
. $SCRIPT_DIR/dockerSecrets.sh

echo "Using MongoDB connection string:"
echo $ZuneNetContext__ConnectionString

docker compose down -v
docker compose build
docker compose up -d --remove-orphans
docker compose logs --follow --tail 0
