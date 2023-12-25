variable "hcloud_token" {
  type = string
  sensitive = true
}

variable "ssh_key_fingerprint" {
  type = string
  default = "90:74:64:18:70:93:29:04:99:61:b4:e3:eb:f9:66:bb"
}
