#!/bin/bash -x

curl "http://web_server:5555/urls/encode?value=hello%21" | jq
