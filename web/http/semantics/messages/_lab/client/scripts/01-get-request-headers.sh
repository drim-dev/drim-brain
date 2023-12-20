#!/bin/bash -x

curl -v "http://web_server:5555/headers" | jq
