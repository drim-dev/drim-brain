# Bitcoin

## General

## Docker

docker run --name=bitcoind-node -d \
    --mount type=bind,source=/d/bitcoin,target=/bitcoin/.bitcoin \
    -p 18332:18332 \
    kylemanna/bitcoind

## Links