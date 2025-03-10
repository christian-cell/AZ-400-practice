using AcrWithWebApp.Resources;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;
using Pulumi.AzureNative.Web.Inputs;
using ManagedServiceIdentityType = Pulumi.AzureNative.Web.ManagedServiceIdentityType;
using SkuName = Pulumi.AzureNative.ContainerRegistry.SkuName;
using Pulumi.AzureNative.Authorization;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();
    var projectName = config.Require("projectName");
    var environment = config.Require("env");
    var location = config.Get("location") ?? "northeurope";
    
    var resourceGroup = new ResourceGroup(
        name: $"{projectName}-rg-{environment}",
        location: location
    );

    var containerRegistry = new Registry("pulumiacrchr", new RegistryArgs
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

    var appServicePlan = new AppServicePlan("appServicePlan", new AppServicePlanArgs
    {
        ResourceGroupName = resourceGroup.ResourceGroupData.Name,
        Location = resourceGroup.ResourceGroupData.Location,
        Name = $"{projectName}-{environment}-serviceplan",
        Kind = "Linux",
        Reserved = true,
        Sku = new SkuDescriptionArgs
        {
            Tier = "Basic",
            Name = "B1"
        },
    });

    var webApp = new WebApp("pulumi-wa-container", new WebAppArgs
    {        
        ResourceGroupName = resourceGroup.ResourceGroupData.Name,
        Location = resourceGroup.ResourceGroupData.Location,
        ServerFarmId = appServicePlan.Id,
        HttpsOnly = true,
        Name = "pulumi-wa-container",
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
                    Value = "8080",
                },
                new NameValuePairArgs{
                    Name = "DOCKER_REGISTRY_SERVER_URL",
                    Value = containerRegistry.LoginServer.Apply(server => $"https://{server}")
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
            LinuxFxVersion = $"DOCKER|pulumiacrchr.azurecr.io/pulumi-image:latest"
        },
    }, new CustomResourceOptions
    {
        DependsOn = containerRegistry
    });
    
    var subscriptionId = Output.Create(GetClientConfig.InvokeAsync()).Apply(config => config.SubscriptionId);

    var acrPullRoleAssignment = new Pulumi.AzureNative.Authorization.RoleAssignment("acrPullRole", new Pulumi.AzureNative.Authorization.RoleAssignmentArgs
    {
        Scope = containerRegistry.Id, 
        PrincipalId = webApp.Identity.Apply(identity => identity?.PrincipalId ?? ""), 
        RoleDefinitionId = subscriptionId.Apply(id =>$"/subscriptions/{id}/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d"),
        PrincipalType = "ServicePrincipal" 
    });


    var webhookUri = Output.Tuple(webApp.Name, resourceGroup.ResourceGroupData.Name).Apply(async tuple =>
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
    
    var webhookFullUri = webhookUri.Apply(uri => $"{uri}/api/registry/webhook");

    var acrWebhook = new Pulumi.AzureNative.ContainerRegistry.Webhook("acrwebhook", new Pulumi.AzureNative.ContainerRegistry.WebhookArgs
    {
        ResourceGroupName = resourceGroup.ResourceGroupData.Name,
        RegistryName = containerRegistry.Name,
        WebhookName = "acrwebhook",
        Location = location,
        ServiceUri = webhookFullUri,
        Status = "enabled",
        Actions = { "push" },
        Scope = "pulumi-image:latest",
    });
});
