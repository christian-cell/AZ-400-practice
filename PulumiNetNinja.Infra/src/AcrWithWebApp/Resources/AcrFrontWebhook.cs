using Pulumi;
using Pulumi.AzureNative.Web;

namespace AcrWithWebApp.Resources
{
    public class AcrFrontWebhook
    {
        public Pulumi.AzureNative.ContainerRegistry.Webhook Webhook;
        
        public AcrFrontWebhook( Web web, ResourceGroup resourceGroup, Acr containerRegistry , string location, string projectName, string environment )
        {
            Output<string?> webhookUri = Output.Tuple(web.AppData.Name, resourceGroup.ResourceGroupData.Name).Apply(async tuple =>
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
            
            Webhook = new Pulumi.AzureNative.ContainerRegistry.Webhook($"{projectName}{environment}acrwebhook", new Pulumi.AzureNative.ContainerRegistry.WebhookArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                RegistryName = containerRegistry.AzureContainerRegistryData.Name,
                WebhookName = $"{projectName}{environment}acrwebhook",
                Location = location,
                ServiceUri = webhookFullUri,
                Status = "enabled",
                Actions = { "push" },
                Scope = "pulumi-image:latest",
            });
        }
        
    }
};

