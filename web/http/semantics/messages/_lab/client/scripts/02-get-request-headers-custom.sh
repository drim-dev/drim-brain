#!/bin/bash -x

curl -v -H "X-CustomHeader: CustomValue" "http://web_server:5555/headers" | jq
