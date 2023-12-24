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

module "api_server_1" {
  source = "./modules/api-server"

  name = "api1"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
}

module "api_server_2" {
  source = "./modules/api-server"

  name = "api2"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
}

module "worker_server_1" {
  source = "./modules/worker-server"

  name = "worker1"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
  volume_size = 40
}

module "worker_server_2" {
  source = "./modules/worker-server"

  name = "worker2"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
  volume_size = 40
}

module "worker_server_3" {
  source = "./modules/worker-server"

  name = "worker3"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
  volume_size = 40
}

module "worker_server_4" {
  source = "./modules/worker-server"

  name = "worker4"
  location = "hel1"
  server_type = "cx21"
  image = "ubuntu-22.04"
  ssh_keys = [data.hcloud_ssh_key.ssh_key.name]
  network_id = hcloud_network.network.id
  volume_size = 40
}

resource "hcloud_load_balancer" "load_balancer" {
  name               = "load_balancer"
  load_balancer_type = "lb11"
  location           = "hel1"
}

resource "hcloud_load_balancer_target" "load_balancer_target_1" {
  type             = "server"
  load_balancer_id = hcloud_load_balancer.load_balancer.id
  server_id        = module.api_server_1.id
}

resource "hcloud_load_balancer_target" "load_balancer_target_2" {
  type             = "server"
  load_balancer_id = hcloud_load_balancer.load_balancer.id
  server_id        = module.api_server_2.id
}
