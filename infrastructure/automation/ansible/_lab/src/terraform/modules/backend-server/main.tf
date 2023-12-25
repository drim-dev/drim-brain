resource "hcloud_server" "backend_server" {
  name        = var.name
  server_type = var.server_type
  image       = var.image
  location    = var.location
  ssh_keys    = var.ssh_keys

  network {
    network_id = var.network_id
  }

  firewall_ids = var.firewall_ids

  labels = {
    purpose = "backend"
  }
}
