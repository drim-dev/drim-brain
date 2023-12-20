# HTTP Messages

HTTP messages are how data is exchanged between a server and a client. There are two types of messages: __requests__ sent by the client to trigger an action on the server, and __responses__, the answer from the server.

![Requests & Responses](_images/requests-responses.png)

## Request

__HTTP request__ is a message sent by a client to a server to initiate a specific action or request a certain resource. HTTP is the foundation of data communication on the World Wide Web. The request consists of several components, including a request method, headers, and, in some cases, a message body. Below is an overview of the main sections of an HTTP request:

1. __Request Line__:
* The request line contains the HTTP method, the target URI (Uniform Resource Identifier), and the HTTP version.
* Example:
```http
GET /example/path HTTP/1.1
```

2. __Request Headers__:
* Headers provide additional information about the request or the client itself.
* Example headers:
```http
Host: www.example.com
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
```

3. __Empty Line__:
* A blank line that separates the headers from the message body.

4. __Request Body (Optional)__:
* The message body carries data associated with the request (e.g., for POST or PUT requests).
* Example (for a POST request):
```http
POST /example/path HTTP/1.1
Host: www.example.com
Content-Type: application/json

{"key": "value"}
```

Here's a breakdown of the components in the above example:

* __HTTP Method__: `POST` indicates that the client is requesting the server to accept and store the enclosed data.
* __URI (Uniform Resource Identifier)__: `/example/path` is the path to the resource on the server.
* __HTTP Version__: `HTTP/1.1` specifies the version of the HTTP protocol being used.
* __Headers__: `Host` specifies the domain name of the server, `Content-Type` indicates the media type of the resource in the body.
`Body`: In this example, it's a JSON payload.

Some HTTP methods include:

* __GET__: Retrieve information from the server.
* __POST__: Submit data to be processed to a specified resource.
* __PUT__: Update a resource on the server.
* __DELETE__: Request the removal of a resource.
* __HEAD__: Retrieve the headers of a resource without the body.

These methods, along with the headers and body, allow for a flexible and powerful communication protocol on the web.

## Response

__HTTP response__ is a message sent by a server to a client in response to an HTTP request. Like the request, the response consists of several sections, each providing specific information about the server's handling of the request. Below is an overview of the main sections of an HTTP response:

1. __Status Line__:

* The status line contains the HTTP version, a status code, and a brief textual description of the status.
* Example:
```http
HTTP/1.1 200 OK
```

2. __Response Headers__:
* Headers provide additional information about the response, the server, or the data being sent.
* Example headers:
```http
Content-Type: text/html; charset=utf-8
Content-Length: 1234
Server: Apache/2.4.41 (Unix)
```

3. __Empty Line__:
* A blank line that separates the headers from the message body.

4. __Response Body (Optional)__:
* The message body carries the actual data or content requested by the client.
* Example (with HTML content):
```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Content-Length: 1234

<!DOCTYPE html>
<html>
<head>
  <title>Example Page</title>
</head>
<body>
  <h1>Hello, World!</h1>
</body>
</html>
```

Here's a breakdown of the components in the above example:

* __HTTP Version__: `HTTP/1.1` specifies the version of the HTTP protocol being used.
* __Status Code__: `200` indicates a successful response. Status codes are three-digit numbers grouped into five classes: _informational (1xx)_, _success (2xx)_, _redirection (3xx)_, _client errors (4xx)_, and _server errors (5xx)_.
* __Status Text__: `OK` is a short, human-readable description of the status code.
* __Headers__: `Content-Type` specifies the media type of the response body, `Content-Length` indicates the size of the content, and `Server` provides information about the server software.
* __Body__: In this example, it's an HTML document.

HTTP responses convey crucial information about the success or failure of a request, and the content or resource requested by the client is typically found in the response body. The status code and headers provide additional context and control over how the client should interpret and handle the response.

## HTTP/1.1 vs HTTP/2

HTTP messages are composed of textual information encoded in ASCII, and span over multiple lines. In HTTP/1.1, and earlier versions of the protocol, these messages were openly sent across the connection. In HTTP/2, the once human-readable message is now divided up into HTTP frames, providing optimization and performance improvements.

![HTTP/1.1 vs HTTP/2](_images/http2.png)

## Links

* https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages

#http-messages
