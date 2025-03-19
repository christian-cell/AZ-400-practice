using AcrWithWebApp.Configs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi;

namespace AcrWithWebApp.Resources
{
    public class Api
    {
        public WebApp AppData { get; }
        
        public Api( ApiWebAppConfiguration config )
        {
            AppData = new WebApp($"{config.ProjectName}-api-{config.Environment}", new WebAppArgs
            {        
                ResourceGroupName = config.ResourceGroup.Name,
                Location = config.ResourceGroup.Location,
                ServerFarmId = config.PlanApi.PlanData.Id,
                HttpsOnly = true,
                Name = $"{config.ProjectName}-api-{config.Environment}",
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
                            Value = config.SqlConnectionString,
                        },
                        new NameValuePairArgs
                        {
                            Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                            Value = config.StorageAccountKey,
                        },
                        new NameValuePairArgs{
                            Name = "DOCKER_REGISTRY_SERVER_URL",
                            Value = config.ContainerRegistry.AzureContainerRegistryData.LoginServer.Apply(server => $"https://{server}")
                        },
                        new NameValuePairArgs
                        {
                            Name = "DOCKER_ENABLE_CI",
                            Value = "true"
                        }
                    },
                    AlwaysOn = true,
                    ManagedPipelineMode = ManagedPipelineMode.Integrated,
                    AcrUseManagedIdentityCreds = true,
                    LinuxFxVersion = $"DOCKER|pulumiacrchr.azurecr.io/pulumi-image:latest",
                    
                    Cors = new CorsSettingsArgs
                    {
                        AllowedOrigins = new[]
                        {
                            "http://localhost:4200/",
                            "http://localhost:4201/",
                            "http://localhost:4202/",
                            $"https://{config.ProjectName.ToLower()}-{config.Environment}.azurewebsites.net/"
                        },
                        SupportCredentials = false,
                    }
                }
                
            }, new CustomResourceOptions
            {
                DependsOn = config.ContainerRegistry.AzureContainerRegistryData
            });
        }
    }
};

