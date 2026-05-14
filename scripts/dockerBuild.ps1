# Import secrets as environment variables
. $PSScriptRoot\dockerSecrets.ps1

echo "Using MongoDB connection string:"
echo $env:ZuneNetContext__ConnectionString

docker compose down -v
docker compose build --no-cache
docker compose up -d --remove-orphans
docker compose logs --follow --tail 0
