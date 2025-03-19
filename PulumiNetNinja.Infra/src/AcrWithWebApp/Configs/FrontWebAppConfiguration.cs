using AcrWithWebApp.Resources;
using ResourceGroup = Pulumi.AzureNative.Resources.ResourceGroup;

namespace AcrWithWebApp.Configs
{
    public class FrontWebAppConfiguration: BaseConfiguration
    {
         public ResourceGroup ResourceGroup { get; set; }
         public PlanFront PlanFront { get; set; }
         public Acr ContainerRegistry { get; set; }
         public string storageAccountKey { get; set; }
         public string sqlConnectionString { get; set; }
    }
};

