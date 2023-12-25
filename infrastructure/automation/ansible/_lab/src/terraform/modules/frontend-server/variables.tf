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

variable "ssh_keys" {
  type = list(string)
}

variable "firewall_ids" {
  type = list(string)
}

variable "network_id" {
  type = number
}
