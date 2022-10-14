terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "~> 2.22.0"
    }
  }
}

provider "docker" {}

resource "docker_image" "docker" {
  name         = "docker/getting-started:latest"
  keep_locally = false
}

resource "docker_container" "nginx" {
  image = docker_image.docker.image_id
  name  = "cool_container"
  ports {
    internal = 80
    external = 80
  }
}
