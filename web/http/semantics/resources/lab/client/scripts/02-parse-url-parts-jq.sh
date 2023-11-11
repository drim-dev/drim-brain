#!/bin/bash -x

curl "http://web_server:5555/urls/some/path?parameter1=value1&parameter2=value2" | jq
