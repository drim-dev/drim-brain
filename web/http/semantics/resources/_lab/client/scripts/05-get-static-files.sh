#!/bin/bash -x

curl -v "http://web_server:5555/static/index.html"

curl -v --output /tmp/dotnet.png "http://web_server:5555/static/images/dotnet.png"

curl -v --output /tmp/dotnet.image1 "http://web_server:5555/static/images/dotnet.image1"

curl -v --output /tmp/dotnet.image2 "http://web_server:5555/static/images/dotnet.image2"

curl -v --output /tmp/dotnet.image3 "http://web_server:5555/static/images/dotnet.image3"
