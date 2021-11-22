# Atomic

develop and deploy all my applications based on [Dapr](https://dapr.io/)

## Folder structure

- applications: source code for each service or web application
- components: building blocks config for dapr
- contracts: dto definition for each language
- deployments: deployment config for dapr and k8s
- infrastructures: shared modules for each language
- scripts: util scripts to improve develop experience

## Scheduled applications

- UnifiedAuth Web
  - [ ] implement OAuth2.0 to provide token and authorization
  - [ ] unified login page for all ui-based applications that require authentication
- Identity Api
  - [x] manage users
  - [x] check, change, reset password
  - [x] check login with password or external provider
  - [ ] manage external login providers
  - [ ] manage permissions
- IdentityServer Api
  - [ ] manage IdentityServer resources
    - [ ] clients
    - [ ] identity resources
    - [ ] api scopes
    - [ ] api resources
  - [ ] allow authenticated user to issue token
- Localization Api
  - [ ] provide unified localization resource among applications
  - [ ] manage supported languages
  - [ ] manage localization resource
- Localization Web
  - [ ] add or update localization materials
  - [ ] manage supported languages
  - [ ] notify when new localization materials should be update or add
  - [ ] allow public contribution to localization
