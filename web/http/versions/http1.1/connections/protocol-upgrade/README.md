# Protocol Upgrade Mechanism

The HTTP/1.1 protocol provides a special mechanism that can be used to upgrade an already established connection to a different protocol, using the `Upgrade` header field.

In practice, this mechanism is used mostly to bootstrap a WebSockets connection.

The HTTP Upgrade mechanism is also used to establish HTTP/2 starting from plain HTTP. It is not a common scenario though.

> Note also that HTTP/2 explicitly disallows the use of this mechanism; it is specific to HTTP/1.1.

## `Upgrade` Header

The `Upgrade` header field is used by clients to invite the server to switch to one of the listed protocols, in descending preference order.

Because `Upgrade` is a hop-by-hop header, it also needs to be listed in the `Connection` header field. This means that a typical request that includes `Upgrade` would look something like:

```http
GET /index.html HTTP/1.1
Host: www.example.com
Connection: upgrade
Upgrade: example/1, foo/2
```

If the server decides to upgrade the connection, it sends back a `101 Switching Protocols` response status with an `Upgrade` header that specifies the protocol(s) being switched to. If it does not (or cannot) upgrade the connection, it ignores the `Upgrade` header and sends back a regular response (for example, a `200 OK`).

Right after sending the `101` status code, the server can begin speaking the new protocol, performing any additional protocol-specific handshakes as necessary. Effectively, the connection becomes a two-way pipe as soon as the upgraded response is complete, and the request that initiated the upgrade can be completed over the new protocol.

## Links

* https://developer.mozilla.org/en-US/docs/Web/HTTP/Protocol_upgrade_mechanism

#http-protocol-upgrade-mechanism
