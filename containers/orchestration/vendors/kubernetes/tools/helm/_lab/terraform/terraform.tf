provider "helm" {
  kubernetes {
    config_path = "../k3s_kubeconfig.yaml"
  }
}
