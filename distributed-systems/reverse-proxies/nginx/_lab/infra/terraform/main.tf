locals {
  frontend_ip = "10.0.1.1"
  backend_ip  = "10.0.1.2"
}

data "hcloud_ssh_key" "primary-ssh-key" {
  fingerprint = "90:74:64:18:70:93:29:04:99:61:b4:e3:eb:f9:66:bb"
}

resource "hcloud_network" "network" {
  name     = "network"
  ip_range = "10.0.0.0/16"
}

resource "hcloud_network_subnet" "subnet" {
  type         = "cloud"
  network_id   = hcloud_network.network.id
  network_zone = "eu-central"
  ip_range     = "10.0.1.0/24"
}

resource "hcloud_firewall" "firewall-shared" {
  name = "firewall-shared"

  rule {
    destination_ips = []
    direction       = "in"
    port            = "22"
    protocol        = "tcp"
    source_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "53"
    protocol        = "tcp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "53"
    protocol        = "udp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "80"
    protocol        = "tcp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "80"
    protocol        = "udp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "443"
    protocol        = "tcp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    direction       = "out"
    port            = "443"
    protocol        = "udp"
    destination_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }
}

resource "hcloud_firewall" "firewall-frontend" {
  name = "firewall-frontend"

  rule {
    destination_ips = []
    direction       = "in"
    port            = "80"
    protocol        = "tcp"
    source_ips      = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "80"
    protocol        = "udp"
    source_ips      = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "443"
    protocol        = "tcp"
    source_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "443"
    protocol        = "udp"
    source_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8080"
    protocol        = "tcp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8080"
    protocol        = "udp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8081"
    protocol        = "tcp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8081"
    protocol        = "udp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }
}

module "frontend-server" {
  source       = "./modules/frontend-server"
  name         = "frontend-server"
  location     = "hel1"
  server_type  = "cax11"
  network_id   = hcloud_network.network.id
  ip           = local.frontend_ip
  firewall_ids = [hcloud_firewall.firewall-shared.id, hcloud_firewall.firewall-frontend.id]
  ssh_keys     = [data.hcloud_ssh_key.primary-ssh-key.name]
}

resource "hcloud_firewall" "firewall-backend" {
  name = "firewall-backend"

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8080"
    protocol        = "tcp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8080"
    protocol        = "udp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8081"
    protocol        = "tcp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "8081"
    protocol        = "udp"
    source_ips      = [
      "${local.frontend_ip}/32",
    ]
  }
}

module "backend-server" {
  source       = "./modules/backend-server"
  name         = "backend-server"
  location     = "hel1"
  server_type  = "cax11"
  network_id   = hcloud_network.network.id
  ip           = local.backend_ip
  firewall_ids = [hcloud_firewall.firewall-shared.id, hcloud_firewall.firewall-backend.id]
  ssh_keys     = [data.hcloud_ssh_key.primary-ssh-key.name]
}
