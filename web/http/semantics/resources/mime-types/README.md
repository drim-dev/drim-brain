# MIME Types

__Multipurpose Internet Mail Extensions__.

A __media type__ indicates the nature and format of a document, file, or bunch of bytes.

A MIME type most commonly consists of just two parts: a __type__ and a __subtype__, separated by a slash (/) — with no whitespace between:

```
type/subtype
```

The type represents the general category into which the data type falls, such as `video` or `text`.

The subtype identifies the exact kind of data of the specified type the MIME type represents. For example, for the MIME type `text`, the subtype might be `plain` (plain text), `html` (HTML source code), or `calendar`(for iCalendar/.ics) files.

An optional __parameter__ can be added to provide additional details:

```
type/subtype;parameter=value
```

For example, for any MIME type whose main type is `text`, you can add the optional `charset` parameter to specify the character set used for the characters in the data. If no `charset` is specified, the default is ASCII (US-ASCII) unless overridden by the user agent's settings. To specify a UTF-8 text file, the MIME type `text/plain;charset=UTF-8` is used.

MIME types are case-insensitive but are traditionally written in lowercase. The parameter values can be case-sensitive.

IANA @iana is responsible for all official MIME types, and you can find the most up-to-date and complete list at their [Media Types](https://www.iana.org/assignments/media-types/media-types.xhtml) page.

## Types

There are two classes of type: __discrete__ and __multipart__. Discrete types are types which represent a single file or medium, such as a single text or music file, or a single video. A multipart type represents a document that's comprised of multiple component parts, each of which may have its own individual MIME type; or, a multipart type may encapsulate multiple files being sent together in one transaction. For example, multipart MIME types are used when attaching multiple files to an email.

https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_Types

### Discrete Types

The discrete types currently registered with the IANA are:

* `application`
* `audio`
* `example`
* `font`
* `image`
* `model`
* `text`
* `video`

### Multipart Types

Multipart types indicate a category of document broken into pieces, often with different MIME types; they can also be used — especially in email scenarios — to represent multiple, separate files which are all part of the same transaction. They represent a __composite document__.

Except for `multipart/form-data`, used in the POST method of HTML Forms, and `multipart/byteranges`, used with `206 Partial Content` to send part of a document, HTTP doesn't handle multipart documents in a special way: the message is transmitted to the browser (which will likely show a "Save As" window if it doesn't know how to display the document).

There are two multipart types:

* `message`
* `multipart`

## MIME Sniffing

In the absence of a MIME type, or in certain cases where browsers believe they are incorrect, browsers may perform _MIME sniffing_ — guessing the correct MIME type by looking at the bytes of the resource.

Each browser performs MIME sniffing differently and under different circumstances. (For example, Safari will look at the file extension in the URL if the sent MIME type is unsuitable.)

## Other Methods Of Conveying Document Types

MIME types are not the only way to convey document type information:

* Filename suffixes are sometimes used, especially on Microsoft Windows. Not all operating systems consider these suffixes meaningful (such as Linux and macOS), and there is no guarantee they are correct.

* Magic numbers. The syntax of different formats allows file-type inference by looking at their byte structure. For example, GIF files start with the `47 49 46 38 39` hexadecimal value (`GIF89`), and PNG files with `89 50 4E 47` (`.PNG`). Not all file types have magic numbers, so this is not 100% reliable either.

## Common Types

The following two important MIME types are the default types:

* `text/plain` is the default value for textual files. A textual file should be human-readable and must not contain binary data.

* `application/octet-stream` is the default value for all other cases. An unknown file type should use this type. Browsers are particularly careful when manipulating these files to protect users from software vulnerabilities and possible dangerous behavior.

[List of MIME types important for the Web](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types)

#mime-types
