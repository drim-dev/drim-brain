data "hcloud_ssh_key" "ssh_key" {
  fingerprint = var.ssh_key_fingerprint
}

resource "hcloud_network" "network" {
  name     = "main_network"
  ip_range = "10.0.0.0/16"
}

resource "hcloud_network_subnet" "subnet" {
  type         = "cloud"
  network_id   = hcloud_network.network.id
  network_zone = "eu-central"
  ip_range     = "10.0.1.0/24"
}

resource "hcloud_firewall" "shared_firewall" {
  name = "shared_firewall"

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

  rule {
    destination_ips = []
    direction       = "in"
    port            = "80"
    protocol        = "tcp"
    source_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }

  rule {
    destination_ips = []
    direction       = "in"
    port            = "80"
    protocol        = "udp"
    source_ips = [
      "0.0.0.0/0",
      "::/0",
    ]
  }
}

module "fronted_server_1" {
  source = "./modules/frontend-server"

  name = "frontend1"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  firewall_ids = [hcloud_firewall.shared_firewall.id]
  network_id = hcloud_network.network.id
}

module "fronted_server_2" {
  source = "./modules/frontend-server"

  name = "frontend2"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  firewall_ids = [hcloud_firewall.shared_firewall.id]
  network_id = hcloud_network.network.id
}

module "backend_server_1" {
  source = "./modules/backend-server"

  name = "backend1"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  firewall_ids = [hcloud_firewall.shared_firewall.id]
  network_id = hcloud_network.network.id
}

module "backend_server_2" {
  source = "./modules/backend-server"

  name = "backend2"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  firewall_ids = [hcloud_firewall.shared_firewall.id]
  network_id = hcloud_network.network.id
}
