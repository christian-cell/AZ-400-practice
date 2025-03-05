using System.Threading.Tasks;
using Users.ContainerApp.Infra.Resources;
using PulumiContainerRegistry = Pulumi.AzureNative.ContainerRegistry;
using ContainerInstance = Pulumi.AzureNative.ContainerInstance;

namespace Users.ContainerApp.Infra
{
    class Program
    {
        static void Main(string[] args)
        {
            Pulumi.Deployment.RunAsync(CreateResourcesAsync).Wait();
        }

        static Task CreateResourcesAsync()
        {
            var config = new Pulumi.Config();
            var projectName = config.Require("projectName");
            var environment = config.Require("env");
            var location = config.Get("location") ?? "northeurope"; 
            var registryName = config.Require("registryName");  

            // 🛠️ resource group
            var resourceGroup = new ResourceGroup(
                $"{projectName}-rg-container-{environment}",
                location: location
            );
            
            var containerGroupName = $"{projectName}-{environment}-containerGroup";

            /* 📒 create container registry */
            var containerRegistry = new PulumiContainerRegistry.Registry(registryName, new PulumiContainerRegistry.RegistryArgs()
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                RegistryName=registryName,
                Sku = new PulumiContainerRegistry.Inputs.SkuArgs
                {
                    Name = PulumiContainerRegistry.SkuName.Basic,
                },
                AdminUserEnabled = true
            });

            /* 📦 create container instance */
            var containerGroup = new ContainerInstance.ContainerGroup(containerGroupName, new ContainerInstance.ContainerGroupArgs()
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                ContainerGroupName = $"{projectName}-{environment}-containerInstance",
                Containers =
                {
                    new ContainerInstance.Inputs.ContainerArgs
                    {
                        Name = $"{projectName}-{environment}-container",
                        Image = "mcr.microsoft.com/azuredocs/aci-helloworld",
                        Resources = new ContainerInstance.Inputs.ResourceRequirementsArgs
                        {
                            Requests = new ContainerInstance.Inputs.ResourceRequestsArgs
                            {
                                Cpu = 1.0,
                                MemoryInGB = 1.5,
                            },
                        },
                    },
                },
                RestartPolicy = ContainerInstance.ContainerGroupRestartPolicy.Always,
                OsType = "Linux",
            });
            return Task.CompletedTask;
        }
    }
};

