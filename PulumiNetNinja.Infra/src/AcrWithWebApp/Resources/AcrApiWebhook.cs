using AcrWithWebApp.Configs;
using Pulumi;
using Pulumi.AzureNative.Web;

namespace AcrWithWebApp.Resources
{
    public class AcrApiWebhook
    {
        public Pulumi.AzureNative.ContainerRegistry.Webhook Webhook;
        
        public AcrApiWebhook( AcrApiWebhookConfiguration config )
        {
            Output<string?> webhookUri = Output.Tuple(config.Api.AppData.Name, config.ResourceGroup.ResourceGroupData.Name).Apply(async tuple =>
            {
                var webAppName = tuple.Item1;
                var resourceGroupName = tuple.Item2;

                var credentials = await ListWebAppPublishingCredentials.InvokeAsync(new ListWebAppPublishingCredentialsArgs
                {
                    Name = webAppName,
                    ResourceGroupName = resourceGroupName
                });

                return credentials.ScmUri;
            });
            
            Output<string> webhookFullUri = webhookUri.Apply(uri => $"{uri}/api/registry/webhook");
            
            Webhook = new Pulumi.AzureNative.ContainerRegistry.Webhook($"{config.ProjectName}api{config.Environment}acrwebhook", new Pulumi.AzureNative.ContainerRegistry.WebhookArgs
            {
                ResourceGroupName = config.ResourceGroup.ResourceGroupData.Name,
                RegistryName = config.ContainerRegistry.AzureContainerRegistryData.Name,
                WebhookName = $"{config.ProjectName}api{config.Environment}acrwebhook",
                Location = config.Location,
                ServiceUri = webhookFullUri,
                Status = "enabled",
                Actions = { "push" },
                Scope = "pulumi-image:latest",
            });
        }
        
    }
};

