resource "helm_release" "redis" {
  name       = "redis"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "redis"
  version    = "19.0.0"
  namespace  = "redis"

  set {
    name  = "master.persistence.enabled"
    value = "false"
  }

  set {
    name  = "replica.persistence.enabled"
    value = "false"
  }
}
