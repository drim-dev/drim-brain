UPDATE "cars" SET "extra_properties"['alarm'] = to_jsonb('Tomahawk 2'::text) WHERE "id" = 1;
