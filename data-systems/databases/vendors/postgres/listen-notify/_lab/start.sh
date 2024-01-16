#!/usr/bin/env bash

docker compose build

docker compose up -d

sleep 1

docker compose exec postgres psql -U test_user -d postgres -f /sql/01-create-database.sql
docker compose exec postgres psql -U test_user -d listen-notify-lab -f /sql/02-create-tables.sql
# docker compose exec postgres psql -U test_user -d listen-notify-lab -f /sql/03-create-trigger.sql
