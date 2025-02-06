locals {
  docker_image          = var.api_docker_image
  aws_access_key        = var.api_access_key_id
  aws_secret_access_key = var.api_secret_access_key
  aws_region            = var.region
}

resource "kubernetes_namespace" "fiap_videoprocessor" {
  metadata {
    name = "fiap-videoprocessor"
  }
}

##############################
# CONFIGS/SECRETS
##############################


resource "kubernetes_config_map_v1" "config_map_api" {
  metadata {
    name      = "configmap-videoprocessor"
    namespace = kubernetes_namespace.fiap_videoprocessor.metadata.0.name
    labels = {
      "app"       = "videoprocessor"
      "terraform" = true
    }
  }
  data = {
    "ASPNETCORE_ENVIRONMENT"               = "Development"
    "Serilog__WriteTo__2__Args__serverUrl" = "http://api-internal.fiap-log.svc.cluster.local"
  }
}

resource "kubernetes_secret" "secret_api" {
  metadata {
    name      = "secret-videoprocessor"
    namespace = kubernetes_namespace.fiap_videoprocessor.metadata.0.name
    labels = {
      app         = "api-pod"
      "terraform" = true
    }
  }
  data = {
    "AWS_SECRET_ACCESS_KEY" = local.aws_secret_access_key
    "AWS_ACCESS_KEY_ID"     = local.aws_access_key
    "AWS_REGION"            = local.aws_region
  }
  type = "Opaque"
}

####################################
# API
####################################

resource "kubernetes_deployment" "deployment_videoprocessor" {
  depends_on = [
    kubernetes_secret.secret_api,
    kubernetes_config_map_v1.config_map_api
  ]

  metadata {
    name      = "deployment-videoprocessor"
    namespace = kubernetes_namespace.fiap_videoprocessor.metadata.0.name
    labels = {
      app         = "videoprocessor"
      "terraform" = true
    }
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "videoprocessor"
      }
    }
    template {
      metadata {
        name = "pod-videoprocessor"
        labels = {
          app         = "videoprocessor"
          "terraform" = true
        }
      }
      spec {
        automount_service_account_token = false
        container {
          name  = "videoprocessor-container"
          image = local.docker_image
          port {
            name           = "liveness-port"
            container_port = 8080
          }
          port {
            container_port = 80
          }

          image_pull_policy = "IfNotPresent"
          liveness_probe {
            http_get {
              path = "/healthz"
              port = "liveness-port"
            }
            period_seconds        = 10
            failure_threshold     = 3
            initial_delay_seconds = 20
          }
          readiness_probe {
            http_get {
              path = "/healthz"
              port = "liveness-port"
            }
            period_seconds        = 10
            failure_threshold     = 3
            initial_delay_seconds = 10
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "200Mi"
            }
            limits = {
              cpu    = "200m"
              memory = "400Mi"
            }
          }
          env_from {
            config_map_ref {
              name = "configmap-videoprocessor"
            }
          }
          env_from {
            secret_ref {
              name = "secret-videoprocessor"
            }
          }
        }
      }
    }
  }
}

resource "kubernetes_horizontal_pod_autoscaler_v2" "hpa_api" {
  metadata {
    name      = "hpa-videoprocessor"
    namespace = kubernetes_namespace.fiap_videoprocessor.metadata.0.name
  }
  spec {
    max_replicas = 6
    min_replicas = 2
    scale_target_ref {
      api_version = "apps/v1"
      kind        = "Deployment"
      name        = "deployment-videoprocessor"
    }

    metric {
      type = "ContainerResource"
      container_resource {
        container = "videoprocessor-container"
        name      = "cpu"
        target {
          average_utilization = 65
          type                = "Utilization"
        }
      }
    }

    metric {
      type = "ContainerResource"
      container_resource {
        container = "videoprocessor-container"
        name      = "memory"
        target {
          average_utilization = 75
          type                = "Utilization"
        }
      }
    }
  }
}
