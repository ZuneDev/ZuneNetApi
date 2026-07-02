$env:DOMAIN_ROOT = "zune.net"
$env:SSL_ACME_EMAIL = "test@example.com"

$env:MONGO_INITDB_ROOT_USERNAME = "root"
$env:MONGO_INITDB_ROOT_PASSWORD = "ChangeMe"

$env:ZuneNetContext__ConnectionString = "mongodb://" + $env:MONGO_INITDB_ROOT_USERNAME + ":" + $env:MONGO_INITDB_ROOT_PASSWORD + "@mongodb:27017/?authSource=admin"
$env:ZuneNetContext__DatabaseName = "Zune"
