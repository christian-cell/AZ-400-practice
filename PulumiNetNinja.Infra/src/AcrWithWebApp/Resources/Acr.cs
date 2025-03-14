using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;

namespace AcrWithWebApp.Resources
{
    public class Acr
    {
        public Registry AzureContainerRegistryData { get; }

        public Acr(ResourceGroup resourceGroup , string location)
        {
            AzureContainerRegistryData = new Registry("pulumiacrchr", new RegistryArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = location,
                AdminUserEnabled = true,
                RegistryName = "pulumiacrchr",
                Sku = new SkuArgs
                {
                    Name = SkuName.Basic
                }
            });
        }
    }
};

