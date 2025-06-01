#!/bin/sh

export SSL_ACME_EMAIL=postmaster@example.com

export MONGO_INITDB_ROOT_USERNAME=root
export MONGO_INITDB_ROOT_PASSWORD=ChangeMe

export ZuneNetContext__ConnectionString=mongodb://${MONGO_INITDB_ROOT_USERNAME}:${MONGO_INITDB_ROOT_PASSWORD}@mongodb:27017/?authSource=admin
export ZuneNetContext__DatabaseName=Zune