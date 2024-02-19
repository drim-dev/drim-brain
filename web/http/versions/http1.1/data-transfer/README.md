# HTTP/1.1 Data Transfer

`Content-Length` and `Transfer-Encoding: chunked` are main HTTP headers used to specify how the body of an HTTP message is being transferred.

## `Content-Length` Header

`Content-Length`` header is a crucial component of the HTTP protocol used to specify the size of the payload body in bytes. It indicates to the recipient the exact length of the message body, allowing the recipient to accurately determine the end of the message and avoid reading beyond the specified content length.

```http
HTTP/1.1 200 OK
Date: Fri, 19 Feb 2024 12:00:00 GMT
Server: Apache/2.4.6 (Unix) OpenSSL/1.1.1
Content-Type: text/html; charset=UTF-8
Content-Length: 104

<!DOCTYPE html>
<html>
<head>
    <title>Example Page</title>
</head>
<body>
    <h1>Hello, World!</h1>
    <p>This is an example page.</p>
</body>
</html>
```

## Chunked Transfer Encoding

__Chunked encoding__ is a transfer coding mechanism by which a web server or a web client sends data in a series of "chunks." Each chunk is preceded by its size in bytes, allowing the receiver to know how much data to expect. This is particularly useful when the total size of the content is not known beforehand or when the data is being generated dynamically.

Chunked encoding is commonly used when the server or the client needs to start sending the response before it has finished generating the entire content, or when the content is being streamed in real-time. It enables browsers and other HTTP clients to start processing and displaying the received data as soon as possible, rather than waiting for the entire response to be received.

Example response:

```http
HTTP/1.1 200 OK
Content-Type: text/plain
Transfer-Encoding: chunked

20\r\n
This is the first chunk of data.\r\n
19\r\n
Here is the second chunk.\r\n
E\r\n
And the third.\r\n
0\r\n
\r\n
```

## Other Transfer Encodings

Other transfer encodings include:

* __compress__. A format using the Lempel-Ziv-Welch (LZW) algorithm. The value name was taken from the UNIX compress program, which implemented this algorithm. Like the compress program, which has disappeared from most UNIX distributions, this content-encoding is used by almost no browsers today, partly because of a patent issue (which expired in 2003).

* __deflate__. Using the zlib structure (defined in RFC 1950), with the deflate compression algorithm (defined in RFC 1951).

* __gzip__. A format using the Lempel-Ziv coding (LZ77), with a 32-bit CRC. This is originally the format of the UNIX gzip program. The HTTP/1.1 standard also recommends that the servers supporting this content-encoding should recognize x-gzip as an alias, for compatibility purposes.

```http
Transfer-Encoding: compress
Transfer-Encoding: deflate
Transfer-Encoding: gzip

// Several values can be listed, separated by a comma
Transfer-Encoding: gzip, chunked
```

#http1.1-data-transfer
