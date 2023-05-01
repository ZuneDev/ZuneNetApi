#!/bin/bash
docker-compose stop
docker-compose build
# if the images aren't getting refreshed, make sure to run docker-compose up first
docker-compose up --no-start --prune-orphans --restart unless-stopped
docker-compose start