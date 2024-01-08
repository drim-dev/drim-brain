# nginx

Nginx is a popular open-source web server and reverse proxy server. It is known for its high performance, stability, and low resource usage. Nginx is commonly used to serve static content, proxy requests to application servers, and act as a load balancer.

Key features of Nginx include its ability to handle a large number of simultaneous connections, efficient use of system resources, and support for various protocols such as HTTP, HTTPS, SMTP, and more. It is widely used in conjunction with popular web technologies and is known for its reliability in delivering web content.

## Proxy Pass

The `proxy_pass` directive is commonly used in the configuration to forward client requests to another server. This is often used in reverse proxy setups or load balancing scenarios.

```nginx
worker_processes 4;
pid /run/nginx.pid;

events {
    worker_connections 4096;
}

http {
    access_log /var/log/nginx/access.log;
    error_log /var/log/nginx/error.log;

    server {
        listen 80;
        server_name frontend.drim.city;

        location / {
            proxy_pass http://10.0.1.1/;
        }

        location /api/ {
            # backend server address
            proxy_pass http://10.0.1.2/;
            
            # setup forwarding headers
            proxy_set_header X-Forwarded-Host $host;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

#nginx
