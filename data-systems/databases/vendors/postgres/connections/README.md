# PostgreSQL Connections

## Listening Addresses Management

To configure PostgreSQL to listen on a specific IP address, you need to modify the `postgresql.conf` file. Follow these steps:

1. __Locate PostgreSQL configuration file__:

The `postgresql.conf` file is usually located in the PostgreSQL data directory. Common paths include `/etc/postgresql/{version}/main/` on Linux or `C:\Program Files\PostgreSQL\{version}\data` on Windows.

2. __Create or edit `postgresql.auto.conf`__:

Create or edit `postgresql.auto.conf` file in a text editor with administrative privileges. This file overrides parameters in `postgresql.conf` file.

3. __Create or find `listen_addresses` section__:

Look for the `listen_addresses` parameter in the file. It may be commented out (with a `#` at the beginning of the line). If it's commented, uncomment the line.

4. __Configure IP address__:

Set the `listen_addresses` parameter to the IP address you want PostgreSQL to listen on. If you want it to listen on all available IP addresses, set it to `'*'`.

Example:

```ini
listen_addresses = '192.168.1.100'      # comma-separated list of addresses
```

or for all addresses:

```ini
listen_addresses = '*'      # Listen on all available IP addresses
```

5. __Save and close file__:

Save the changes and close the `postgresql.auto.conf` file.

6. __Restart PostgreSQL__:

After making these changes, you need to restart the PostgreSQL server for the new configuration to take effect.
On Linux, you can use:

```sh
sudo service postgresql restart
```

On Windows, you can use the Services application to restart the PostgreSQL service.

## Client Access Management

1. __Locate file `pg_hba.conf`__:

It is in the same directory as `postgresql.conf`.

2. __Add access rule__:

Add line `host db_name role_name client_ip/32 scram-sha-256` to `pg_hba.conf`, where:

* `db_name` is the name of the DB to allow connections to.

* `role_name` is the name of role to allow connections for.

* `client_ip` is IP address of the host to allow connections from.

3. __Save and close file__.

4. __Restart PostgreSQL__.

## Links

* https://www.postgresql.org/docs/current/runtime-config-connection.html
* https://www.postgresql.org/docs/current/auth-pg-hba-conf.html

#postgresql-connections
