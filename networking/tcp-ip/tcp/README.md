# TCP

The __Transmission Control Protocol__ (__TCP__) is one of the main protocols of the TCP/IP stack. TCP provides __reliable__, __ordered__, and __error-checked__ delivery of a __stream of octets__ (bytes) between applications running on hosts communicating via an IP network. Major internet applications such as the World Wide Web, email, remote administration, and file transfer rely on TCP. SSL/TLS often runs on top of TCP.

TCP is __connection-oriented__, and a connection between client and server is established before data can be sent. The server must be listening (passive open) for connection requests from clients before a connection is established.

At the transport layer, TCP handles all handshaking and transmission details and presents an abstraction of the network connection to the application typically through a __network socket__ interface.

At the lower levels of the protocol stack, due to network congestion, traffic load balancing, or unpredictable network behaviour, IP packets may be lost, duplicated, or delivered out of order. TCP detects these problems, requests re-transmission of lost data and rearranges out-of-order data.

## Links

* https://en.wikipedia.org/wiki/Transmission_Control_Protocol
* https://codeburst.io/understanding-tcp-internals-step-by-step-for-software-engineers-system-designers-part-1-df0c10b86449

#tcp
