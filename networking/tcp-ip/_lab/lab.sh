#!/usr/bin/env bash

docker compose build

docker compose run --rm client

docker compose down
