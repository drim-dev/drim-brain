# PostgreSQL

SHOW shared_buffers;

SHOW ALL;

ALTER SYSTEM SET work_mem = '500MB';

SELECT pg_reload_conf();
