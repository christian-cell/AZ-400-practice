using AcrWithWebApp.Configs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi;

namespace AcrWithWebApp.Resources
{
    public class Web
    {
        public WebApp AppData { get; }
        public Output<string> AppUrl { get; }
        
        public Web( FrontWebAppConfiguration config )
        {
            AppData = new WebApp($"{config.ProjectName}-{config.Environment}", new WebAppArgs
            {        
                ResourceGroupName = config.ResourceGroup.Name,
                Location = config.ResourceGroup.Location,
                ServerFarmId = config.PlanFront.PlanData.Id,
                HttpsOnly = true,
                Name = $"{config.ProjectName}-{config.Environment}",
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
                            Value = "80",
                        },
                        new NameValuePairArgs{
                            Name = "DOCKER_REGISTRY_SERVER_URL",
                            Value = config.ContainerRegistry.AzureContainerRegistryData.LoginServer.Apply(server => $"https://{server}")
                        },
                        new NameValuePairArgs
                        {
                            Name = "DOCKER_ENABLE_CI",
                            Value = "true"
                        },                
                    },
                    AlwaysOn = true,
                    ManagedPipelineMode = ManagedPipelineMode.Integrated,
                    AcrUseManagedIdentityCreds = true,
                    LinuxFxVersion = $"DOCKER|pulumiacrchr.azurecr.io/pulumi:latest"
                },
            }, new CustomResourceOptions
            {
                DependsOn = config.ContainerRegistry.AzureContainerRegistryData
            });
            
            AppUrl = Output.Format($"https://{AppData.DefaultHostName}");
        }
    }
};

