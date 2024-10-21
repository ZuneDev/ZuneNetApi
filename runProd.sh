#!/bin/bash
docker-compose stop
docker-compose build
docker-compose start #??? --prune-orphans