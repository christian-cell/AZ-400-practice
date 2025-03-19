using AcrWithWebApp.Resources;
using ResourceGroup = Pulumi.AzureNative.Resources.ResourceGroup;

namespace AcrWithWebApp.Configs
{
    public class ApiWebAppConfiguration: BaseConfiguration
    {
        public ResourceGroup ResourceGroup { get; set; }
        public PlanApi PlanApi { get; set; }
        public Acr ContainerRegistry { get; set; }
        public Pulumi.Output<string> StorageAccountKey { get; set; }
        public Pulumi.Output<string> SqlConnectionString { get; set; }
    }
};

