locals {
  docker_image          = var.api_docker_image
  aws_access_key        = var.api_access_key_id
  aws_secret_access_key = var.api_secret_access_key
  aws_region            = var.region
}

##############################
# CONFIGS/SECRETS
##############################


resource "kubernetes_config_map_v1" "config_map_api" {
  metadata {
    name      = "configmap-videoprocessor"
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
    labels = {
      app         = "api-pod"
      "terraform" = true
    }
  }
  data = {
    "AWS_SECRET_ACCESS_KEY"  = local.aws_secret_access_key
    "AWS_ACCESS_KEY_ID"      = local.aws_access_key
    "AWS_REGION"             = local.aws_region
  }
  type = "Opaque"
}

####################################
# API
####################################

# resource "kubernetes_service" "videoprocessor-svc" {
#   metadata {
#     name      = "api-internal"
#     annotations = {
#       "service.beta.kubernetes.io/aws-load-balancer-type"   = "nlb"
#       "service.beta.kubernetes.io/aws-load-balancer-scheme" = "internal"
#     }
#   }
#   spec {
#     port {
#       port        = 80
#       target_port = 8080
#       node_port   = 30007
#       protocol    = "TCP"
#     }
#     type = "LoadBalancer"
#     selector = {
#       app : "videoprocessor"
#     }
#   }
# }

resource "kubernetes_deployment" "deployment_videoprocessor" {
  depends_on = [
    kubernetes_secret.secret_api,
    kubernetes_config_map_v1.config_map_api
  ]

  metadata {
    name      = "deployment-videoprocessor"
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
              memory = "120Mi"
            }
            limits = {
              cpu    = "150m"
              memory = "200Mi"
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
  }
  spec {
    max_replicas = 5
    min_replicas = 3
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
  }
}