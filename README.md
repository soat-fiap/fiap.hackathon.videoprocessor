# Video Processor

This repository is part of a project focused on processing videos to extract images. It includes several key features and integrations to ensure efficient video processing and high code quality.

## Key Features
- **Video Processing**: Handles the extraction of images from video files, including frame capture and image storage.
- **CI/CD Integration**: Utilizes GitHub Actions for continuous integration and deployment, ensuring that changes are automatically tested and deployed.
  - [![Build and Deploy](https://github.com/soat-fiap/fiap.hackathon.videoprocessor/actions/workflows/build-and-deploy.yaml/badge.svg)](https://github.com/soat-fiap/fiap.hackathon.videoprocessor/actions/workflows/build-and-deploy.yaml)
- **Code Quality Assurance**: Integrates with SonarCloud to monitor code quality and coverage, helping maintain high standards.
  - ![Coverage](https://sonarcloud.io/api/project_badges/measure?project=soat-fiap_fiap.hackathon.videoprocessor&metric=coverage)
- **Collaborative Design**: Includes a link to an Event Storming board on Miro, indicating a collaborative approach to designing the system.
  - [Event Storm](https://miro.com/app/board/uXjVL3XY9ds=/)

## Repository Structure
- **Application Code**: Contains the core logic for video processing, including APIs, frame extraction, and image storage modules.
- **Tests**: Includes unit and integration tests to ensure the reliability and correctness of the video processing functionalities.
- **Infrastructure as Code**: Uses Terraform or similar tools to manage infrastructure, ensuring consistent and reproducible environments.

This repository is essential for processing videos and extracting images, providing a robust and scalable solution for video processing within the project.

### Services Communication (ECST)
![image](comunicacao_servicos-HACKATHON.drawio.png)