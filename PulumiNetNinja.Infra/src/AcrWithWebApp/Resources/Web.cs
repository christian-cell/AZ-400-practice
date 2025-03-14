using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi;

namespace AcrWithWebApp.Resources
{
    public class Web
    {
        public WebApp AppData { get; }
        public Output<string> AppUrl { get; }
        
        public Web( ResourceGroup resourceGroup , PlanFront appServicePlanFront , Acr containerRegistry, string projectName , string environment )
        {
            AppData = new WebApp($"{projectName}-{environment}", new WebAppArgs
            {        
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = resourceGroup.ResourceGroupData.Location,
                ServerFarmId = appServicePlanFront.PlanData.Id,
                HttpsOnly = true,
                Name = $"{projectName}-{environment}",
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
                            Value = containerRegistry.AzureContainerRegistryData.LoginServer.Apply(server => $"https://{server}")
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
                    LinuxFxVersion = $"DOCKER|pulumiacrchr.azurecr.io/users:latest"
                },
            }, new CustomResourceOptions
            {
                DependsOn = containerRegistry.AzureContainerRegistryData
            });
            
            AppUrl = Output.Format($"https://{AppData.DefaultHostName}");
        }
    }
};

