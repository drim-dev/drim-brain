# NGINX Connection Management

## Connection-Related Settings

In Nginx, connection-related settings are crucial for managing how the server handles incoming connections, timeouts, and other network-related configurations. Here are some key settings related to connections in Nginx:

* `worker_connections`. This directive sets the maximum number of simultaneous connections that can be opened by each worker process. It's typically set in the `events` block of the Nginx configuration file.

```nginx
events {
    worker_connections 1024;
}
```

* `keepalive_timeout`. Specifies the timeout for keep-alive connections. After this period of inactivity, Nginx will close the connection.

```nginx
http {
    keepalive_timeout 65;
}
```

* `keepalive_requests`. Sets the maximum number of requests that can be sent over a single keep-alive connection before it's closed by Nginx.

```nginx
http {
    keepalive_requests 100;
}
```

* `client_body_timeout` and `client_header_timeout`. These directives set the maximum time for reading the request body and request headers, respectively. If the client takes longer than these timeouts to send the request body or headers, Nginx closes the connection.

```nginx
http {
    client_body_timeout 10;
    client_header_timeout 10;
}
```

* `send_timeout`. Defines the maximum time Nginx will wait for the response to be sent to the client. If no data is sent to the client within this time, the connection is closed.

```nginx
http {
    send_timeout 10;
}
```

* `limit_conn`. Allows you to limit the number of connections per IP address. This directive is typically used to mitigate various types of attacks such as DDoS (Distributed Denial of Service) attacks or to manage server resources efficiently.

```nginx
http {
    limit_conn_zone $binary_remote_addr zone=conn_limit_per_ip:10m;

    server {
        location / {
            limit_conn conn_limit_per_ip 10;
            ...
        }
    }
}
```

#nginx-connection-management
