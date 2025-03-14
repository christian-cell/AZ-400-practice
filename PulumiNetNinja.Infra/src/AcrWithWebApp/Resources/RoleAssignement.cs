using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace AcrWithWebApp.Resources
{
    public class RoleAssignement
    {
        private readonly  Output<string> _subscriptionId = Output.Create(GetClientConfig.InvokeAsync()).Apply(config => config.SubscriptionId);

        public Pulumi.AzureNative.Authorization.RoleAssignment AcrApiRoleAssignementData;
        public Pulumi.AzureNative.Authorization.RoleAssignment AcrFrontRoleAssignementData;
        public RoleAssignement( Acr containerRegistry , Api api, Web web )
        {
            AcrApiRoleAssignementData = new Pulumi.AzureNative.Authorization.RoleAssignment("apiAcrPullRole", new Pulumi.AzureNative.Authorization.RoleAssignmentArgs
            {
                Scope = containerRegistry.AzureContainerRegistryData.Id, 
                PrincipalId = api.AppData.Identity.Apply(identity => identity?.PrincipalId ?? ""), 
                RoleDefinitionId = _subscriptionId.Apply(id =>$"/subscriptions/{id}/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d"),
                PrincipalType = "ServicePrincipal" 
            });
            
            AcrFrontRoleAssignementData = new Pulumi.AzureNative.Authorization.RoleAssignment("frontAcrPullRole", new Pulumi.AzureNative.Authorization.RoleAssignmentArgs
            {
                Scope = containerRegistry.AzureContainerRegistryData.Id, 
                PrincipalId = web.AppData.Identity.Apply(identity => identity?.PrincipalId ?? ""), 
                RoleDefinitionId = _subscriptionId.Apply(id =>$"/subscriptions/{id}/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d"),
                PrincipalType = "ServicePrincipal" 
            });
        }
    }
};

