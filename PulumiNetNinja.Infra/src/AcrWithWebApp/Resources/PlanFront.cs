using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi;

namespace AcrWithWebApp.Resources
{
    public class PlanFront
    {
        public AppServicePlan PlanData { get; }
        
        public PlanFront( ResourceGroup resourceGroup , string projectName , string environment )
        {
            PlanData = new AppServicePlan($"{projectName}-{environment}-serviceplan", new AppServicePlanArgs
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
        }
    }
};

