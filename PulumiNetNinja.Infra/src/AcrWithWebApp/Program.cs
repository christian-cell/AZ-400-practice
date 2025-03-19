using AcrWithWebApp.Configs;
using AcrWithWebApp.Resources;
using Pulumi;

// SqlConfig
var sqlServerConfig = SqlServerConfig.Load();

return await Pulumi.Deployment.RunAsync(() =>
{
    // Stack Params
    var config = new Pulumi.Config();
    var projectName = config.Require("projectName");
    var environment = config.Require("env");
    var location = config.Get("location") ?? "northeurope";
    
    // 🛠️ resource group
    var resourceGroup = new ResourceGroup(
        name: $"{projectName}-rg-{environment}",
        location: location
    );
    
    // 💾 storage account
    var storageAccount = new StorageAccountResource(
        accountName: $"{projectName}stg{environment}",
        resourceGroup: resourceGroup
    );
    
    // 📇 database
    var sqlServerResource = new SqlServerResource(
        sqlServerConfig: sqlServerConfig,
        resourceGroup: resourceGroup,
        new SqlServerConfiguration
        {
            ServerName = $"{projectName}-sql-{environment}",
            Environment = environment,
            Location = location,
            ProjectName = projectName
        }
    );
    
    // 📈 telemetry

    var logAnalyticsWorkspace = new AnalyticsWorkSpace(
        new BaseConfiguration
        {
            Environment = environment,
            ProjectName = projectName,
            Location = location
        }, 
        resourceGroup
        );
    
    var appInsights = new ApplicationInsights(
        new BaseConfiguration
        {
            Environment = environment,
            ProjectName = projectName,
            Location = location
        }
        ,resourceGroup, logAnalyticsWorkspace
        );
    
    var sqlConnectionString = Output.Format($@"Server={sqlServerResource.SqlServerData.Name}.database.windows.net;Database={sqlServerConfig.DatabaseName}-{environment};User Id={sqlServerConfig.AdminUsername};Password={sqlServerConfig.AdminPassword};");
    var storageAccountKey = storageAccount.PrimaryStorageKey;

    // 🐋 Azure container registry
    var containerRegistry = new Acr( resourceGroup , location );
    
    var apiPlan = new PlanApi(resourceGroup, projectName, environment);
    
    var frontPlan = new PlanFront(resourceGroup, projectName, environment);
    
    var api = new Api(
        new ApiWebAppConfiguration()
        {
            ResourceGroup = resourceGroup.ResourceGroupData,
            PlanApi = apiPlan,
            ContainerRegistry = containerRegistry,
            ProjectName = projectName,
            Environment = environment,
            StorageAccountKey = storageAccountKey,
            SqlConnectionString = sqlConnectionString
        }
        );
    
    var web = new Web(
        new FrontWebAppConfiguration()
        {
            ResourceGroup = resourceGroup.ResourceGroupData,
            PlanFront = frontPlan,
            ContainerRegistry = containerRegistry,
            ProjectName = projectName,
            Environment = environment
        }
        );
    
    
    var roleAssignement = new RoleAssignement( containerRegistry , api , web );
    
    var acrApiWebhook = new AcrApiWebhook(
        new AcrApiWebhookConfiguration()
        {
            Api = api,
            ContainerRegistry = containerRegistry,
            ResourceGroup = resourceGroup,
            ProjectName = projectName,
            Environment = environment,
            Location = location
        }
        );
   
    
    var acrFrontWebhook = new AcrFrontWebhook(
        new AcrFrontWebhookConfiguration()
        {
            Web = web,
            ContainerRegistry = containerRegistry,
            ResourceGroup = resourceGroup,
            ProjectName = projectName,
            Environment = environment,
            Location = location
        }
        );
    
    var appRegistration = new AppRegistration(
        new AppRegistrationConfiguration()
        {
            ProjectName = projectName,
            Environment = environment,
            Web = web 
        }
        );
});


