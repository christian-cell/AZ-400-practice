using Pulumi.AzureNative.Insights;

namespace AcrWithWebApp.Resources
{
    public class ApplicationInsights
    {
        public Component ApplicationInsightsData;

        public ApplicationInsights( string projectName, string environment, ResourceGroup resourceGroup, string location, AnalyticsWorkSpace logAnalyticsWorkspace)
        {
            ApplicationInsightsData = new Component($"{projectName}-insights-{environment}", new ComponentArgs
            {
                ResourceName = $"{projectName}-insights-{environment}",
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = location,
                ApplicationType = "web",
                Kind = "web",
                WorkspaceResourceId = logAnalyticsWorkspace.AnalyticsWorkSpaceData.Id.Apply(id => id) 
            });
        }
    }
};

