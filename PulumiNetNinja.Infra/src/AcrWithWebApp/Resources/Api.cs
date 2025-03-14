using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi;

namespace AcrWithWebApp.Resources
{
    public class Api
    {
        public WebApp AppData { get; }
        
        public Api( ResourceGroup resourceGroup , PlanApi appServicePlanApi , Acr containerRegistry, string projectName, string environment,  Output<string> storageAccountKey,  Output<string> sqlConnectionString )
        {
            AppData = new WebApp($"{projectName}-api-{environment}", new WebAppArgs
            {        
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = resourceGroup.ResourceGroupData.Location,
                ServerFarmId = appServicePlanApi.PlanData.Id,
                HttpsOnly = true,
                Name = $"{projectName}-api-{environment}",
                Identity = new ManagedServiceIdentityArgs
                {
                    Type = ManagedServiceIdentityType.SystemAssigned,
                },
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings =
                    {
                        new NameValuePairArgs
                        {
                            Name = "WEBSITES_PORT",
                            Value = "8080",
                        },
                        new NameValuePairArgs
                        {
                            Name = "ConnectionStrings__DefaultConnection",
                            Value = sqlConnectionString,
                        },
                        new NameValuePairArgs
                        {
                            Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                            Value = storageAccountKey,
                        },
                        new NameValuePairArgs{
                            Name = "DOCKER_REGISTRY_SERVER_URL",
                            Value = containerRegistry.AzureContainerRegistryData.LoginServer.Apply(server => $"https://{server}")
                        },
                        new NameValuePairArgs
                        {
                            Name = "DOCKER_ENABLE_CI",
                            Value = "true"
                        }/*,
                        new NameValuePairArgs
                        {
                            Name = "FUNCTIONS_WORKER_RUNTIME",
                            Value = Output.Create("dotnet-isolated")
                        },*/
                    },
                    AlwaysOn = true,
                    ManagedPipelineMode = ManagedPipelineMode.Integrated,
                    AcrUseManagedIdentityCreds = true,
                    LinuxFxVersion = $"DOCKER|pulumiacrchr.azurecr.io/users-api:latest",
                    
                    Cors = new CorsSettingsArgs
                    {
                        AllowedOrigins = new[]
                        {
                            "http://localhost:4200/",
                            "http://localhost:4201/",
                            "http://localhost:4202/",
                            $"https://{projectName.ToLower()}-{environment}.azurewebsites.net/"
                        },
                        SupportCredentials = false,
                    }
                }
                
            }, new CustomResourceOptions
            {
                DependsOn = containerRegistry.AzureContainerRegistryData
            });
        }
    }
};

