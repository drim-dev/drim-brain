resource "hcloud_server" "frontend" {
  name        = var.name
  server_type = var.server_type
  image       = var.image
  location    = var.location
  ssh_keys    = var.ssh_keys

  network {
    network_id = var.network_id
    ip         = var.ip
  }

  firewall_ids = var.firewall_ids

  labels = {
    purpose = "frontend"
  }
}
