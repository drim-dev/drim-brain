job "web-api" {

  group "web-api" {
    count = 3
    network {
      port "http" {
        to = 80
      }
    }

    service {
      name = "web-api"
      port = "http"

      check {
        type     = "http"
        path     = "/health"
        interval = "10s"
        timeout  = "2s"

        check_restart {
          limit = 3
          grace = "20s"
        }
      }
    }

    task "web-api" {
      driver = "docker"

      config {
        image = "web-api:nomad"
        ports = ["http"]
      }
    }
  }
}
