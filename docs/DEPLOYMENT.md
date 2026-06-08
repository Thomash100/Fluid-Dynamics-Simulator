# Deployment

Pipeline:

Push -> Pull Request -> Tests -> Build -> Tag -> Release -> Raspberry Pi Deployment

Release tags:

- v0.1.0-alpha

Required GitHub Secrets:

- RPI_HOST
- RPI_USER
- RPI_SSH_KEY

Target directory:

/opt/fluid-dynamics-simulator
