-- DDL
CREATE DATABASE "CryptoBank";

CREATE TABLE "Users" (
	"Id" INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	"Email" VARCHAR(64) UNIQUE NOT NULL,
	"PasswordHash" VARCHAR(256) NOT NULL,
	"DateOfBirth" TIMESTAMP NOT NULL,
	"RegisteredAt" TIMESTAMP NOT NULL
);

CREATE TABLE "Xpubs" (
	"Id" INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	"CurrencyCode" VARCHAR(16) NOT NULL,
	"Value" VARCHAR(256) NOT NULL
);

CREATE TABLE "DepositAddresses" (
	"Id" INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	"CurrencyCode" VARCHAR(16) NOT NULL,
    "DerivationIndex" INT NOT NULL CHECK("DerivationIndex" >= 0),
    "Value" VARCHAR(256) NOT NULL,
    "XpubId" INT NOT NULL REFERENCES "Xpubs" ("Id"),
    "UserId" INT NOT NULL REFERENCES "Users" ("Id")
);

CREATE TABLE "CryptoDeposits" (
    "Id" BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "CurrencyCode" VARCHAR(16) NOT NULL,
    "Amount" DECIMAL NOT NULL,
    "TxId" VARCHAR(64) NOT NULL,
    "Confirmations" INT NOT NULL,
    "Status" SMALLINT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "ConfirmedAt" TIMESTAMP NULL,
    "UserId" INT NOT NULL REFERENCES "Users" ("Id"),
    "AddressId" INT NOT NULL REFERENCES "DepositAddresses" ("Id")
);

-- DML
INSERT INTO "Users" VALUES(DEFAULT, 'sam@cryptobank.com', 'hash1', '1995-01-30', '2023-06-29');
INSERT INTO "Users" VALUES(DEFAULT, 'tom@cryptobank.com', 'hash2', '1997-02-17', '2023-06-30');
INSERT INTO "Users" VALUES(DEFAULT, 'jack@cryptobank.com', 'hash3', '2001-06-05', '2023-07-01');
INSERT INTO "Users" VALUES(DEFAULT, 'john@gmail.com', 'hash4', '2000-06-05', '2023-07-02');

INSERT INTO "Xpubs" VALUES(DEFAULT, 'BTC', 'xpub123456');
INSERT INTO "Xpubs" VALUES(DEFAULT, 'BTC', 'xpubabcdef');

INSERT INTO "DepositAddresses" VALUES(DEFAULT, 'BTC', 1, 'bc0123456', 1, 1);

SELECT * FROM "Users";

DELETE FROM "Users" WHERE "Email" = 'sam@cryptobank.com';

SELECT * FROM "Users";

UPDATE "Users" SET "PasswordHash" = 'updated_hash' WHERE "Id" = 2;

SELECT * FROM "Users";

SELECT * FROM "Users" WHERE "Email" LIKE '%@cryptobank.com';
SELECT * FROM "Users" WHERE "RegisteredAt" < '2023-07-01';

-- Test data
INSERT INTO "Users" ("Email", "PasswordHash", "DateOfBirth", "RegisteredAt")
SELECT
    CONCAT(md5(random()::text), md5(random()::text)),
    md5(random()::text),
    NOW(),
    NOW()
FROM generate_series(1, 100000);

INSERT INTO "DepositAddresses" ("CurrencyCode", "DerivationIndex", "Value", "XpubId", "UserId")
SELECT
    'BTC',
    1,
    md5(random()::text),
    1,
    random() * 100000 + 1
FROM generate_series(1, 10000000);

-- Foreign key violation
INSERT INTO "DepositAddresses" VALUES(DEFAULT, 'BTC', 2, 'bcabcdef', 1, 100);

-- Check constraint violation
INSERT INTO "DepositAddresses" VALUES(DEFAULT, 'BTC', -1, 'bcabcdef', 1, 1);

-- Joins
SELECT *
FROM "DepositAddresses" a
INNER JOIN "Users" u ON a."UserId" = u."Id"
INNER JOIN "Xpubs" x ON a."XpubId" = x."Id";

SELECT a."Value", u."Email", x."Value" as "XpubValue"
FROM "DepositAddresses" a
INNER JOIN "Users" u ON a."UserId" = u."Id"
INNER JOIN "Xpubs" x ON a."XpubId" = x."Id";

-- Performance
SELECT * FROM "DepositAddresses" WHERE "Id" = 2000000;
EXPLAIN SELECT * FROM "DepositAddresses" WHERE "Id" = 2000000;
EXPLAIN ANALYZE SELECT * FROM "DepositAddresses" WHERE "Id" = 2000000;

SELECT * FROM "DepositAddresses" WHERE "UserId" = 90000;
EXPLAIN SELECT * FROM "DepositAddresses" WHERE "UserId" = 90000;
EXPLAIN ANALYZE SELECT * FROM "DepositAddresses" WHERE "UserId" = 90000;

CREATE INDEX IX_DepositAddresses_UserId ON "DepositAddresses" ("UserId");

SELECT * FROM "DepositAddresses" WHERE "UserId" = 90000;

SELECT "Value", "DerivationIndex" FROM "DepositAddresses" WHERE "UserId" = 90000;

CREATE INDEX IX_DepositAddresses_UserId_Covering ON "DepositAddresses" ("UserId") INCLUDE ("Value", "DerivationIndex");

SELECT "Value", "DerivationIndex" FROM "DepositAddresses" WHERE "UserId" = 90000;
