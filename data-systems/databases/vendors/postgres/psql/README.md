# PostgreSQL `psql`

`psql` is a command-line interface (CLI) tool for interacting with PostgreSQL databases. It allows users to connect to a PostgreSQL database server, run SQL queries, and perform various database operations.

## Commands

1. __Connect to PostgreSQL database with password__:

```sh
psql -h host -p port -d database -U username -W
```

2. __Connect to database__:

```sh
\c db_name
```

3. __List databases__:

```sh
\l
```

4. __List tables__:

```sh
\dt
```

5. __Describe table__:

```sh
\d table_name
```

6. __Enable or disable query timing__:

```sh
\timing [on|off]
```

7. __Enable or disable expanded display mode__:

```sh
\x [on|off|auto]
```

8. __Exit__:

```sh
\q
```

## Links

* https://www.postgresql.org/docs/current/app-psql.html

#postgresql-psql
