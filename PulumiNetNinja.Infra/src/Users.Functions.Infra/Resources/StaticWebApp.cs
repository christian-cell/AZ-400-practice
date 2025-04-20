using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;

namespace Users.Functions.Infra.Resources
{
    public class StaticWebApp
    {
        
        public StaticWebApp(
            ResourceGroup resourceGroup,
            string location
            )
        {
            var staticSite = new StaticSite("my-static-app", new StaticSiteArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Location = "westeurope",
                Sku = new SkuDescriptionArgs
                {
                    Name = "Free",
                    Tier = "Free",
                },
                RepositoryUrl = "https://github.com/christian-cell/AZ-400-practice",
                Branch = "master",
                BuildProperties = new StaticSiteBuildPropertiesArgs
                {
                    AppLocation = "/",
                    OutputLocation = "build"
                }
            });

            var dnsZone = new Zone("users-functions-dev.com", new ZoneArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                ZoneName = "users-functions-dev.com",
                Location = "global"
            });

            var cnameRecord = new RecordSet("static-site-cname", new RecordSetArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                ZoneName = dnsZone.Name,
                RelativeRecordSetName = "www",
                RecordType = "CNAME",
                Ttl = 3600,
                CnameRecord = new CnameRecordArgs
                {
                    Cname = staticSite.DefaultHostname
                }
            });

            this.Endpoint = staticSite.DefaultHostname;
        }

        public Output<string> Endpoint { get; private set; }
    
    }
};

