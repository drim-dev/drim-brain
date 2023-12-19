# Encryption

__Encryption__ is a process used to secure and protect information by converting it into a coded form that can only be deciphered by authorized individuals or systems. There are two main types of encryption: __symmetric encryption__ and __asymmetric encryption__.

## Symmetric Encryption

* __Key Concept__: In symmetric encryption, the same key is used for both the encryption and decryption of the data.
* __Process__: The sender and the receiver must both have a copy of the shared key, and this key is used to encrypt and decrypt the data.
* __Advantages__: Symmetric encryption is generally faster and less computationally intensive than asymmetric encryption.
* __Disadvantages__: The challenge lies in securely sharing and managing the secret key, especially in situations where secure key exchange is difficult.
* __Algorithms__: AES, Blowfish, Serpent.

![Symmetric Encryption](_images/symmetric-encryption.png)

## Asymmetric Encryption

* __Key Concept__: Asymmetric encryption uses a pair of keys, a public key, and a private key. The public key is used for encryption, and the private key is used for decryption.
* __Process__:
  * The public key can be freely distributed and is used by anyone who wants to send an encrypted message to the owner of the private key.
  * The private key is kept secret and is used by the owner to decrypt messages encrypted with their public key.
* __Advantages__: Asymmetric encryption provides a solution to the key distribution problem inherent in symmetric encryption. Even if the public key is widely known, only the private key can decrypt messages.
* __Disadvantages__: Asymmetric encryption is generally slower than symmetric encryption due to the complexity of the algorithms involved.
* __Algorithms__: RSA, ECC, ElGamal.

![Asymmetric Encryption](_images/asymmetric-encryption.png)

In practice, a common approach is to use a combination of both symmetric and asymmetric encryption in what is known as hybrid encryption. In this approach, symmetric encryption is used for the actual data, and asymmetric encryption is used to secure and exchange the symmetric keys. This combines the efficiency of symmetric encryption with the key management advantages of asymmetric encryption.

#encryption
