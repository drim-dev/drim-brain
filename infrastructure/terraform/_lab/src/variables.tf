variable "hcloud_token" {
  type = string
  sensitive = true
}

variable "ssh_key_fingerprint" {
  type = string
  default = "6d:8f:78:4e:5f:ba:d9:d0:d5:24:97:1c:a9:c2:95:d9"
}
