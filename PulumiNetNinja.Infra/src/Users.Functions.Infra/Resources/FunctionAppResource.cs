using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Users.Functions.Infra.Configs;

namespace Users.Functions.Infra.Resources
{
    public class FunctionAppResource
    {
        public WebApp FunctionApp { get; } 

        public FunctionAppResource(
            string functionName,
            FunctionAppConfig functionAppConfig,
            ShareConfig shareConfig,
            string environment,
            ResourceGroup resourceGroup,
            Dictionary<string, Output<string>>? appSettings = null)
        {
            var appServicePlan = new AppServicePlan($"{functionName}-serviceplan",
                new AppServicePlanArgs
                {
                    Kind = "FunctionApp", 
                    Location = shareConfig.Location,
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    Sku = new SkuDescriptionArgs
                    {
                        Name = "Y1", 
                        Tier = "Dynamic"
                    },
                    Name = $"{functionName}-serviceplan"
                }
            );

            FunctionApp = new WebApp(functionName, new WebAppArgs
            {
                Location = shareConfig.Location,
                Name = functionName,
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                ServerFarmId = appServicePlan.Id,
                Kind = "functionapp",
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = appSettings?.Select(kvp => new NameValuePairArgs
                    {
                        Name = kvp.Key,
                        Value = kvp.Value
                    }).ToArray()!
                }
            });
        }
    }
}