job "postgres" {
  datacenters = ["local"]
  type = "service"

  group "postgres" {
    count = 1

    volume "postgres" {
      type = "host"
      read_only = false
      source = "postgres"
    }

    restart {
      attempts = 10
      interval = "5m"
      delay    = "25s"
      mode     = "delay"
    }

    task "postgres" {
      driver = "docker"

      volume_mount {
        volume      = "postgres"
        destination = "/var/lib/postgresql/data"
        read_only   = false
      }

      env = {
        "POSTGRES_USER" = "db_creator"
        "POSTGRES_PASSWORD" = "12345678"
      }

      config {
        image = "stellirin/postgres-windows"

        ports = ["db"]
      }

      resources {
        cpu    = 500
        memory = 1024
      }

      service {
        name = "postgres"
        port = "db"

        // check {
        //   type     = "tcp"
        //   interval = "10s"
        //   timeout  = "2s"
        // }
      }
    }

    network {
      port "db" {
        to = 5432
        static = 15432
      }
    }
  }
}