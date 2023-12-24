resource "hcloud_firewall" "worker_firewall" {
  name = "${var.name}_server"

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

resource "hcloud_server" "worker_server" {
  name        = var.name
  server_type = var.server_type
  image       = var.image
  location    = var.location
  ssh_keys    = var.ssh_keys

  network {
    network_id = var.network_id
  }

  public_net {
    ipv4_enabled = false
    ipv6_enabled = false
  }

  firewall_ids = [
    hcloud_firewall.worker_firewall.id
  ]

  labels = {
    purpose = "worker"
  }
}

resource "hcloud_volume" "worker_volume" {
  name              = "${var.name}_volume"
  size              = var.volume_size
  server_id         = hcloud_server.worker_server.id
  automount         = true
  format            = "ext4"
  delete_protection = false

  labels = {
    purpose = "worker"
  }
}
