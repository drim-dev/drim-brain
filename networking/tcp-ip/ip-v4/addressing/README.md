# IPv4 Addressing

An Internet Protocol address (IP address) is a numerical label such as 192.0.2.1 that is connected to a computer network that uses the Internet Protocol for communication. An IP address serves two main functions:

* _network interface identification_
* _location addressing_.

Internet Protocol version 4 (IPv4) defines an IP address as a 32-bit number. However, because of the growth of the Internet and the depletion of available IPv4 addresses, a new version of IP (IPv6), using 128 bits for the IP address, was standardized in 1998. IPv6 deployment has been ongoing since the mid-2000s.

An IP address serves two principal functions: it identifies the host, or more specifically its network interface, and it provides the location of the host in the network, and thus the capability of establishing a path to that host. Its role has been characterized as follows:

> A name indicates what we seek. An address indicates where it is. A route indicates how to get there.

The header of each IP packet contains the IP address of the sending host and that of the destination host.

IP address structure:

![IP address](_images/ip-address.png)

## CIDR

__Classless Inter-Domain Routing__ is a method for allocating IP addresses and for IP routing.

IP addresses are described as consisting of two groups of bits in the address: the most significant bits are the __network prefix__, which identifies a whole network or subnet, and the least significant set forms the __host identifier__, which specifies a particular interface of a host on that network. This division is used as the basis of traffic routing between IP networks and for address allocation policies.

CIDR is based on __variable-length subnet masking__ (__VLSM__), in which network prefixes have variable length. The main benefit of this is that it grants finer control of the sizes of subnets allocated to organizations, hence slowing the exhaustion of IPv4 addresses from allocating larger subnets than needed. CIDR gave rise to a new way of writing IP addresses known as CIDR notation, in which an IP address is followed by a suffix indicating the number of bits of the prefix. Some examples of CIDR notation are the addresses `192.0.2.1/24`, `10.0.13.131/16`, or `3.54.19.40/8`.

A __subnet mask__ is a bitmask that encodes the prefix length associated with an IPv4 address or network in quad-dotted notation: 32 bits, starting with a number of 1-bits equal to the prefix length, ending with 0-bits, and encoded in four-part dotted-decimal format: for example, `255.255.255.0`.

Address     | 192.0.2.1/24
------------|-------------
CIDR range  | 192.0.2.0/24
Netmask     | 255.255.255.0
Host mask   | 0.0.0.255
First IP    | 192.0.2.0
Last IP     | 192.0.2.255
Total hosts | 256

Address     | 10.0.13.131/16
------------|-------------
CIDR range  | 10.0.0.0/16
Netmask     | 255.255.0.0
Host mask   | 0.0.255.255
First IP    | 10.0.0.0
Last IP     | 10.0.255.255
Total hosts | 65536

Address     | 3.54.19.40/8
------------|-------------
CIDR range  | 3.0.0.0/8
Netmask     | 255.0.0.0
Host mask   | 0.255.255.255
First IP    | 3.0.0.0
Last IP     | 3.255.255.255
Total hosts | 16777216

## Special-use Addresses

The Internet Engineering Task Force (IETF) and IANA have restricted from general use various reserved IP addresses for special purposes. Notably these addresses are used for _multicast traffic_ and to provide addressing space for unrestricted uses on _private networks_.

Address block      | Address range                  | Number of addresses | Scope           | Description
-------------------|--------------------------------|---------------------|-----------------|--------------------------------
0.0.0.0/8          | 0.0.0.0–0.255.255.255          | 16777216            | Software        | Current (local, "this") network
10.0.0.0/8         | 10.0.0.0–10.255.255.255        | 16777216            | Private network | Used for local communications within a private network
100.64.0.0/10      |	100.64.0.0–100.127.255.255  | 4194304             | Private network | Shared address space for communications between a service provider and its subscribers when using a carrier-grade NAT
127.0.0.0/8        | 127.0.0.0–127.255.255.255      | 16777216            | Host            | Used for loopback addresses to the local host
169.254.0.0/16     | 169.254.0.0–169.254.255.255    | 65536               | Subnet          | Used for link-local addresses between two hosts on a single link when no IP address is otherwise specified, such as would have normally been retrieved from a DHCP server.
172.16.0.0/12      | 172.16.0.0–172.31.255.255      | 1048576             | Private network | Used for local communications within a private network
192.0.0.0/24       | 192.0.0.0–192.0.0.255          | 256                 | Private network | IETF Protocol Assignments, DS-Lite (/29)
192.0.2.0/24       | 192.0.2.0–192.0.2.255          | 256                 | Documentation   | Assigned as TEST-NET-1, documentation and examples
192.88.99.0/24     | 192.88.99.0–192.88.99.255      | 256                 | Internet        | Reserved. Formerly used for IPv6 to IPv4 relay (included IPv6 address block 2002::/16)
192.168.0.0/16     | 192.168.0.0–192.168.255.255    | 65536               | Private network | Used for local communications within a private network
198.18.0.0/15      | 198.18.0.0–198.19.255.255      | 131072              | Private network | Used for benchmark testing of inter-network communications between two separate subnets
198.51.100.0/24    | 198.51.100.0–198.51.100.255    | 256                 | Documentation   | Assigned as TEST-NET-2, documentation and examples
203.0.113.0/24     | 203.0.113.0–203.0.113.255      | 256                 | Documentation   | Assigned as TEST-NET-3, documentation and examples
224.0.0.0/4        | 224.0.0.0–239.255.255.255      | 268435456           | Internet        | In use for IP multicast. (Former Class D network)
233.252.0.0/24     | 233.252.0.0-233.252.0.255      | 256                 | Documentation   | Assigned as MCAST-TEST-NET, documentation and examples
240.0.0.0/4        | 240.0.0.0–255.255.255.254      | 268435455           | Internet        | Reserved for future use. (Former Class E network)
255.255.255.255/32 | 255.255.255.255                | 1                   | Subnet          | Reserved for the "limited broadcast" destination address

### Private Networks

Of the approximately four billion addresses defined in IPv4, about 18 million addresses in three ranges are reserved for use in _private networks_. Packets addresses in these ranges are not routable in the public Internet; they are ignored by all public routers. Therefore, private hosts cannot directly communicate with public networks, but require __network address translation__ (__NAT__) at a routing gateway for this purpose.

Name         | CIDR block     | Address range                 | Number of addresses | Classful description
-------------|----------------|-------------------------------|---------------------|---------------------
24-bit block | 10.0.0.0/8     | 10.0.0.0 – 10.255.255.255     | 16777216            | Single Class A
20-bit block | 172.16.0.0/12  | 172.16.0.0 – 172.31.255.255   | 1048576             | Contiguous range of 16 Class B blocks
16-bit block | 192.168.0.0/16 | 192.168.0.0 – 192.168.255.255 | 65536               | Contiguous range of 256 Class C blocks.

## NAT

TODO

## Links

* https://en.wikipedia.org/wiki/IP_address
* https://en.wikipedia.org/wiki/Classless_Inter-Domain_Routing
* https://www.ipaddressguide.com/cidr

#ipv4-addressing
