1 Create host agent

![alt text](readmeImages/click-project-settings.png)
![alt text](readmeImages/add-pools.png)
![alt text](readmeImages/self-hosted-agent.png)
![alt text](readmeImages/self-hosted-parallel-jobs.png)

follow instructions to create agent locally and connect it by devops token

2 Create a pipeline
you need to add your host agent in the .yml

![alt text](readmeImages/create-pipeline.png)
![alt text](readmeImages/from-azure-repos.png)
![alt text](readmeImages/select-repository.png)
![alt text](readmeImages/existing-yml.png)
![alt text](readmeImages/select-branch-yml.png)

3 Create service connection

you need to create a app registration in azure and save:
- CLIENT ID
- TENANT ID
- SECRET
  ![alt text](readmeImages/service-connection.png)
  ![alt text](readmeImages/get-your-appregistration.png)

4 Create variable group in the library

![alt text](readmeImages/create-var-group.png)
![alt text](readmeImages/add-permissions-for-pipelines.png)

- add the value
- as a secret
- or from a keyvault

5 Enable release for your project in organization level

![alt text](readmeImages/adding-releases-to-project.png)
![alt text](readmeImages/enable-releases.png)

6 Create release

![alt text](readmeImages/create-release.png)
![alt text](readmeImages/create-release-from-azure-service.png)
![alt text](readmeImages/release-from-pipeline.png)
![alt text](readmeImages/enable-run-after-pipeline.png)
![alt text](readmeImages/add-job-and-approvers.png)
![alt text](readmeImages/select-app-service.png)
![alt text](readmeImages/choose-your-agent.png)
![alt text](readmeImages/add-new-task.png)
![alt text](readmeImages/file-transform-task.png)
![alt text](readmeImages/drag-task-here.png)
![alt text](readmeImages/load-artifact.png)
![alt text](readmeImages/link-variable-group.png)

use it with "." or with ":" 
sample : Azure.ApplicationInsights.InstrumentationKey
![alt text](readmeImages/define-appsettings-replacement.png)
![alt text](readmeImages/set-config.png)

use this option if your app read vars from env vars and not from appsettings.json
![alt text](readmeImages/env-vars.png)

save and create release







