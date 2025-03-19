using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using System.IO;
using System.Text.Json;
using AcrWithWebApp.Configs;

namespace AcrWithWebApp.Resources
{
    public class AnalyticsWorkSpace
    {
        public Workspace AnalyticsWorkSpaceData;

        public AnalyticsWorkSpace( BaseConfiguration config, ResourceGroup resourceGroup )
        {
            AnalyticsWorkSpaceData = new Workspace($"{config.ProjectName}-insights-{config.Environment}", 
                new WorkspaceArgs
                {
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    Location = config.Location,
                    Sku = new WorkspaceSkuArgs
                    {
                        Name = "PerGB2018"
                    },
                    WorkspaceName = $"{config.ProjectName}-insights-{config.Environment}",
                    RetentionInDays = 30
                },
                new CustomResourceOptions
                {
                    DependsOn = new[] { resourceGroup.ResourceGroupData } 
                });
            
            UpdateAppSettingsWithWorkspaceResourceId(AnalyticsWorkSpaceData.Id);
        }
        
        void UpdateAppSettingsWithWorkspaceResourceId(Output<string> workspaceResourceId)
        {
            workspaceResourceId.Apply(resourceId =>
            {
                var filePath = "appsettings.json";

                var json = File.Exists(filePath) ? File.ReadAllText(filePath) : "{}";
                var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();

                if (!config.ContainsKey("ApplicationInsightsConfig"))
                {
                    config["ApplicationInsightsConfig"] = new Dictionary<string, object>();
                }

                var appInsightsConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(config["ApplicationInsightsConfig"])) 
                                        ?? new Dictionary<string, object>();
                appInsightsConfig["AppInsightsName"] = appInsightsConfig.ContainsKey("AppInsightsName") ? appInsightsConfig["AppInsightsName"] : string.Empty;
                appInsightsConfig["WorkspaceResourceId"] = resourceId;
                config["ApplicationInsightsConfig"] = appInsightsConfig;

                File.WriteAllText(filePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

                return resourceId; 
            });
        }
    }
};

