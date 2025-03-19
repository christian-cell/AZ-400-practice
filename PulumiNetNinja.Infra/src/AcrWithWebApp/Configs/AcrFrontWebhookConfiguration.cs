using AcrWithWebApp.Resources;

namespace AcrWithWebApp.Configs
{
    public class AcrFrontWebhookConfiguration: BaseConfiguration
    {
        public Web Web { get; set; }
        public ResourceGroup ResourceGroup { get; set; }
        public Acr ContainerRegistry { get; set; }
    }
};

