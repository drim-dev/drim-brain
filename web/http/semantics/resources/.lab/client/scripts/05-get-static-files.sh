#!/bin/bash -x

curl "http://web_server:5555/static/index.html"

curl --output /tmp/dotnet.png "http://web_server:5555/static/images/dotnet.png"
