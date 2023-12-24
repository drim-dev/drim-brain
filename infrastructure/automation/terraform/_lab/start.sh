#!/usr/bin/env bash

docker compose build

docker compose run terraform-client
