using AcrWithWebApp.Configs;
using Pulumi.AzureNative.Insights;

namespace AcrWithWebApp.Resources
{
    public class ApplicationInsights
    {
        public Component ApplicationInsightsData;

        public ApplicationInsights( BaseConfiguration config,ResourceGroup resourceGroup, AnalyticsWorkSpace logAnalyticsWorkspace)
        {
            ApplicationInsightsData = new Component($"{config.ProjectName}-insights-{config.Environment}", new ComponentArgs
            {
                ResourceName = $"{config.ProjectName}-insights-{config.Environment}",
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = config.Location,
                ApplicationType = "web",
                Kind = "web",
                WorkspaceResourceId = logAnalyticsWorkspace.AnalyticsWorkSpaceData.Id.Apply(id => id) 
            });
        }
    }
};

