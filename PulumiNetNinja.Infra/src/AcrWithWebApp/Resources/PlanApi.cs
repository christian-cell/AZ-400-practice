using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace AcrWithWebApp.Resources
{
    public class PlanApi
    {
        public AppServicePlan PlanData { get; }
        
        public PlanApi( ResourceGroup resourceGroup , string projectName , string environment )
        {
            PlanData = new AppServicePlan($"{projectName}-api-{environment}-serviceplan", new AppServicePlanArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = resourceGroup.ResourceGroupData.Location,
                Name = $"{projectName}-api-{environment}-serviceplan",
                Kind = "Linux",
                Reserved = true,
                Sku = new SkuDescriptionArgs
                {
                    Tier = "Basic",
                    Name = "B1"
                },
            });
        }
    }
};

