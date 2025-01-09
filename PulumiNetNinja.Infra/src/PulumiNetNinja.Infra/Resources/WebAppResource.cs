using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using netNinja.API.infra.Configs;
using Pulumi.AzureNative.Web.Inputs;

namespace netNinja.API.infra.Resources
{
    public class WebAppResource
    {
        public WebApp WebAppData { get; }

        public WebAppResource(string appName, WebAppConfig webAppConfig, ShareConfig shareConfig, string environment, ResourceGroup resourceGroup, Dictionary<string, Output<string>>? appSettings = null)
        {
            var appServicePlan = new AppServicePlan($"{appName}-serviceplan",
                new AppServicePlanArgs
                {
                    Kind = "App", 
                    Location = shareConfig.Location,
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name, 
                    Sku = new SkuDescriptionArgs
                    {
                        Name = webAppConfig.Sku.Size,
                        Tier = webAppConfig.Sku.Tier
                    },
                    Name =  $"{appName}-serviceplan"/*$"{webAppConfig.AppServicePlanName}-{environment}"*/
                }
            );

            WebAppData = new WebApp(appName/*$"{webAppConfig.AppName}-{environment}"*/,
                new WebAppArgs
                {
                    Location = shareConfig.Location,
                    Name = appName/*$"{webAppConfig.AppName}-{environment}"*/,
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    ServerFarmId = appServicePlan.Id,
                    SiteConfig = new SiteConfigArgs
                    {
                        AppSettings = appSettings?.Select(kvp => new NameValuePairArgs
                        {
                            Name = kvp.Key,
                            Value = kvp.Value
                        }).ToArray()!
                    }
                }
            );
        }
    }
};

