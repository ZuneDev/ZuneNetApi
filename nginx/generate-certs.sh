#! /bin/bash
openssl req -x509 -nodes -days 365 -subj "/C=CA/ST=QC/O=Microsoft, Inc./CN=*.zune.net" -newkey rsa:2048 -keyout ./ssl/private/nginx-selfsigned.key -out ./ssl/certs/nginx-selfsigned.crt;