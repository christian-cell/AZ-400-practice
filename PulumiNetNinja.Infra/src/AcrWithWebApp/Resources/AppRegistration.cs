using AcrWithWebApp.Configs;
using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureAD.Inputs;
using Pulumi.AzureNative.NetworkCloud.V20230701;

namespace AcrWithWebApp.Resources
{
    public class AppRegistration
    {
        public Application AppRegistrationData;
        public AppRegistration( AppRegistrationConfiguration config )
        {
            Pulumi.Log.Info($"La URL de la aplicación web es: {config.Web.AppUrl}");
            Pulumi.Log.Info($"La URL de la aplicación web es: {config.Web.AppUrl.ToString()}");
            
            config.Web.AppUrl.Apply(url =>
            {
                Pulumi.Log.Info($"La URL de la aplicación web es: {url}");
                return url;
            });
            
            AppRegistrationData = new Application($"{config.ProjectName}-{config.Environment}-regis", new ApplicationArgs
                {
                    DisplayName = $"{config.ProjectName}-{config.Environment}-regis",
                    SignInAudience = "AzureADMyOrg",  

                    Web = new ApplicationWebArgs
                    {
                        RedirectUris = new[]
                        {
                            $"https://{config.ProjectName.ToLower()}-{config.Environment}.azurewebsites.net/",
                            $"http://localhost:4200/",
                            $"https://{config.ProjectName.ToLower()}-api-{config.Environment}.azurewebsites.net/swagger/index.html"
                        },
                        ImplicitGrant = new ApplicationWebImplicitGrantArgs
                        {
                            AccessTokenIssuanceEnabled = true,  
                            IdTokenIssuanceEnabled = true      
                        }
                    }
                },
                
                new CustomResourceOptions
                {
                    DependsOn = config.Web.AppData
                });
        }
    }
};