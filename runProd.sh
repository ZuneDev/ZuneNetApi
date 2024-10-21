#!/bin/bash
docker compose stop
docker compose build
docker compose up -d --remove-orphans
docker compose logs --follow --tail 0
