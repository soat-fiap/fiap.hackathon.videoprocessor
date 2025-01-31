variable "profile" {
  description = "AWS profile name"
  type        = string
  default     = "default"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "eks_cluster_name" {
  type    = string
  default = "eks_dev_quixada"
}

variable "api_docker_image" {
  type    = string
  default = "ghcr.io/soat-fiap/fiap.hackathon.videoprocessor/api:latest"
}

variable "api_access_key_id" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}

variable "api_secret_access_key" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}