#!/usr/bin/env bash

curl -H "Content-Type: application/json" --data '{ "Name": "Toyota", "Color": "blue", "Year": 2012, "Mileage": 10000 }' -v http://localhost:9000/notify
