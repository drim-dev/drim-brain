CREATE TABLE "brands" (
	"id" SMALLINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	"name" VARCHAR(64) UNIQUE NOT NULL
);

CREATE TABLE "models" (
	"id" SMALLINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "brand_id" SMALLINT NOT NULL REFERENCES "brands" ("id"),
	"name" VARCHAR(64) UNIQUE NOT NULL
);

CREATE TABLE "colors" (
	"id" SMALLINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	"name" VARCHAR(64) UNIQUE NOT NULL
);

CREATE TABLE "cars" (
    "id" INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "model_id" SMALLINT NOT NULL REFERENCES "models" ("id"),
    "production_year" SMALLINT NOT NULL CHECK("production_year" >= 1800 AND "production_year" <= 2200),
    "color_id" SMALLINT NOT NULL REFERENCES "colors" ("id"),
    "mileage" INT NOT NULL CHECK("mileage" >= 0),
    "extra_properties" JSONB NOT NULL
);