#!/bin/sh
set -eu

API_URL="${API_URL:-http://localhost:8080}"
envsubst '${API_URL}' < /usr/share/nginx/html/assets/app-config.template.js > /usr/share/nginx/html/assets/app-config.js

exec nginx -g 'daemon off;'
