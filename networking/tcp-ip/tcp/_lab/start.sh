#!/usr/bin/env bash

docker compose build

docker compose run tcp-client
