using Pulumi.AzureNative.Resources;

namespace Users.Functions.Infra.Resources
{
    public class ResourceGroup
    {
        public Pulumi.AzureNative.Resources.ResourceGroup ResourceGroupData { get; }

        public ResourceGroup(string name, string location)
        {
            ResourceGroupData = new Pulumi.AzureNative.Resources.ResourceGroup(name, new ResourceGroupArgs
            {
                ResourceGroupName = name,
                Location = location,
            });
        }
    }
};