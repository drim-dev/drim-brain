variable "name" {
  type = string
}

variable "location" {
  type = string
}

variable "server_type" {
  type = string
}

variable "image" {
  type = string
  default = "ubuntu-22.04"
}

variable "network_id" {
  type = number
}

variable "ip" {
  type = string
}

variable "firewall_ids" {
  type = list(string)
}

variable "ssh_keys" {
  type = list(string)
}
