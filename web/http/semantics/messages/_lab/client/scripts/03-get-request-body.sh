#!/bin/bash -x

curl -v -H "Content-Type: text/plain" --data "This is my body" "http://web_server:5555/body" | jq
