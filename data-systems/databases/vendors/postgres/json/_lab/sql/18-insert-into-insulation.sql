UPDATE "cars" SET "extra_properties"['insulation'] = "extra_properties" -> 'insulation' || '[ "floor" ]'::jsonb WHERE "id" = 2;
