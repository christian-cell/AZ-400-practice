using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureAD.Inputs;
using Pulumi.AzureNative.NetworkCloud.V20230701;

namespace AcrWithWebApp.Resources
{
    public class AppRegistration
    {
        public Application AppRegistrationData;
        public AppRegistration(string projectName , string environment, Web web)
        {
            Pulumi.Log.Info($"La URL de la aplicación web es: {web.AppUrl}");
            Pulumi.Log.Info($"La URL de la aplicación web es: {web.AppUrl.ToString()}");
            
            web.AppUrl.Apply(url =>
            {
                // Imprimir la URL de la aplicación.
                Pulumi.Log.Info($"La URL de la aplicación web es: {url}");
                return url;
            });
            
            AppRegistrationData = new Application($"{projectName}-{environment}-regis", new ApplicationArgs
                {
                    DisplayName = $"{projectName}-{environment}-regis",
                    SignInAudience = "AzureADMyOrg",  

                    Web = new ApplicationWebArgs
                    {
                        RedirectUris = new[]
                        {
                            $"https://{projectName.ToLower()}-{environment}.azurewebsites.net/",
                            $"http://localhost:4200/",
                            $"https://{projectName.ToLower()}-api-{environment}.azurewebsites.net/swagger/index.html"
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
                    DependsOn = web.AppData
                });
        }
    }
};