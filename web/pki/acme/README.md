# Automatic Certificate Management Environment

Automatic Certificate Management Environment (ACME) is a protocol designed to automate the process of certificate issuance, renewal, and revocation. ACME streamlines the certificate management process by allowing web servers to communicate with a certificate authority (such as Let's Encrypt) to obtain, renew, and revoke certificates automatically, without human intervention. This automation helps ensure that websites maintain up-to-date, valid certificates, enhancing security and privacy for users.

Key features of ACME include:

* __Automated Certificate Issuance__. ACME enables web servers to request and obtain SSL/TLS certificates automatically, eliminating the need for manual certificate generation and installation.

* __Certificate Renewal__. ACME facilitates the automatic renewal of certificates before they expire, helping websites maintain continuous HTTPS connectivity without interruption.

* __Revocation__. ACME allows for the revocation of certificates if they are compromised or no longer needed, helping to maintain security.

* __Protocol Flexibility__: ACME supports different validation methods for proving domain ownership during certificate issuance, including HTTP-01, DNS-01, and TLS-ALPN-01 challenges.

* __Client Integration__: ACME clients are available for various web servers and platforms, making it easy to integrate ACME support into existing infrastructure.

Overall, ACME simplifies the process of managing TLS certificates, making HTTPS encryption more accessible and widely adopted across the internet.

## Let's Encrypt

The Let's Encrypt project was started in 2012 by two Mozilla employees, Josh Aas and Eric Rescorla, together with Peter Eckersley at the Electronic Frontier Foundation and J. Alex Halderman at the University of Michigan. Internet Security Research Group, the company behind Let's Encrypt, was incorporated in May 2013.

The objective of Let’s Encrypt and the ACME protocol is to make it possible to set up an HTTPS server and have it automatically obtain a browser-trusted certificate, without any human intervention. This is accomplished by running a certificate management agent on the web server.

To understand how the technology works, let’s walk through the process of setting up `https://example.com/` with a certificate management agent that supports Let’s Encrypt.

There are two steps to this process. First, the agent proves to the CA that the web server controls a domain. Then, the agent can request, renew, and revoke certificates for that domain.

### Domain Validation

Let’s Encrypt identifies the server administrator by public key. The first time the agent software interacts with Let’s Encrypt, it generates a new key pair and proves to the Let’s Encrypt CA that the server controls one or more domains. This is similar to the traditional CA process of creating an account and adding domains to that account.

To kick off the process, the agent asks the Let’s Encrypt CA what it needs to do in order to prove that it controls `example.com`. The Let’s Encrypt CA will look at the domain name being requested and issue one or more sets of challenges. These are different ways that the agent can prove control of the domain. For example, the CA might give the agent a choice of either:

* Provisioning a DNS record under `example.com`, or
* Provisioning an HTTP resource under a well-known URI on `http://example.com/`

Along with the challenges, the Let’s Encrypt CA also provides a nonce that the agent must sign with its private key pair to prove that it controls the key pair.

![Challenge](_images/challenge.png)

The agent software completes one of the provided sets of challenges. Let’s say it is able to accomplish the second task above: it creates a file on a specified path on the `http://example.com` site. The agent also signs the provided nonce with its private key. Once the agent has completed these steps, it notifies the CA that it’s ready to complete validation.

Then, it’s the CA’s job to check that the challenges have been satisfied. The CA verifies the signature on the nonce, and it attempts to download the file from the web server and make sure it has the expected content.

![Authorization](_images/authorization.png)

If the signature over the nonce is valid, and the challenges check out, then the agent identified by the public key is authorized to do certificate management for `example.com`. We call the key pair the agent used an “authorized key pair” for example.com.

### Certificate Issuance and Revocation

Once the agent has an authorized key pair, requesting, renewing, and revoking certificates is simple—just send certificate management messages and sign them with the authorized key pair.

To obtain a certificate for the domain, the agent constructs a PKCS#10 Certificate Signing Request that asks the Let’s Encrypt CA to issue a certificate for `example.com` with a specified public key. As usual, the CSR includes a signature by the private key corresponding to the public key in the CSR. The agent also signs the whole CSR with the authorized key for example.com so that the Let’s Encrypt CA knows it’s authorized.

When the Let’s Encrypt CA receives the request, it verifies both signatures. If everything looks good, it issues a certificate for example.com with the public key from the CSR and returns it to the agent.

![Certificate](_images/certificate.png)

Revocation works in a similar manner. The agent signs a revocation request with the key pair authorized for `example.com`, and the Let’s Encrypt CA verifies that the request is authorized. If so, it publishes revocation information into the normal revocation channels (i.e. OCSP), so that relying parties such as browsers can know that they shouldn’t accept the revoked certificate.

![Revocation](_images/revocation.png)

## Certbot

Certbot is a free, open-source software tool used for automatically managing SSL/TLS certificates issued by Let's Encrypt, a free certificate authority. It simplifies the process of obtaining, renewing, and configuring HTTPS certificates for websites. Certbot is widely used with web servers like Apache and Nginx and supports various operating systems including Linux distributions such as Ubuntu, Debian, CentOS, and Fedora.

## Challenge Types

There are generally three types of ACME challenges:

* __HTTP-01 challenge__. In this challenge, the ACME server provides a token that needs to be placed in a specific location on the website accessible via HTTP. The server then checks whether the token is present to confirm domain ownership.

* __DNS-01 challenge__: This challenge involves adding a specific TXT record to the domain's DNS zone. The ACME server checks for the presence of this record to verify domain ownership.

* __TLS-ALPN-01__ challenge: This challenge requires the ACME client to prove domain ownership by responding to a challenge sent over a TLS connection.

## Links

* ACME - https://www.thesslstore.com/blog/acme-protocol-what-it-is-and-how-it-works/
* Let's Encrypt - https://letsencrypt.org/
* How Let's Encrypt Works - https://letsencrypt.org/how-it-works/
* RFC 8555 - https://datatracker.ietf.org/doc/html/rfc8555
* RFC 2986 - https://datatracker.ietf.org/doc/html/rfc2986
* Certbot - https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-20-04

#acme
