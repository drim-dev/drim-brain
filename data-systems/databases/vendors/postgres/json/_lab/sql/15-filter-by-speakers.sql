SELECT * FROM "cars" WHERE ("extra_properties" -> 'stereo' ->> 'speakers')::int > 10;
