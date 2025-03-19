using AcrWithWebApp.Resources;

namespace AcrWithWebApp.Configs
{
    public class AcrApiWebhookConfiguration: BaseConfiguration
    {
        public Api Api { get; set; }
        public ResourceGroup ResourceGroup { get; set; }
        public Acr ContainerRegistry { get; set; }
    }
};

