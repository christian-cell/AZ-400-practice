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
        /*storageAccountConfig,*/
        accountName: $"{projectName}stg{environment}",
        resourceGroup: resourceGroup
    );
    
    // 📇 database
    var sqlServerResource = new SqlServerResource(
        sqlServerConfig: sqlServerConfig,
        resourceGroup: resourceGroup,
        environment: environment,
        serverName : $"{projectName}-sql-{environment}",
        projectName: projectName,
        location: location
    );
    
    // 📈 telemetry

    var logAnalyticsWorkspace = new AnalyticsWorkSpace(projectName, environment, location, resourceGroup);
    var appInsights = new ApplicationInsights(projectName, environment, resourceGroup, location, logAnalyticsWorkspace);
    
    var sqlConnectionString = Output.Format($@"Server={sqlServerResource.SqlServerData.Name}.database.windows.net;Database={sqlServerConfig.DatabaseName}-{environment};User Id={sqlServerConfig.AdminUsername};Password={sqlServerConfig.AdminPassword};");
    var storageAccountKey = storageAccount.PrimaryStorageKey;

    
    var containerRegistry = new Acr( resourceGroup , location );
    
    var apiPlan = new PlanApi(resourceGroup, projectName, environment);
    var frontPlan = new PlanFront(resourceGroup, projectName, environment);
    var api = new Api(resourceGroup, apiPlan, containerRegistry, projectName , environment, storageAccountKey, sqlConnectionString);
    var web = new Web(resourceGroup, frontPlan, containerRegistry, projectName , environment);
    var roleAssignement = new RoleAssignement( containerRegistry , api , web );
    var acrApiWebhook = new AcrApiWebhook(api, resourceGroup, containerRegistry, location, projectName, environment);
    var acrFrontWebhook = new AcrFrontWebhook(web, resourceGroup, containerRegistry, location, projectName, environment);
    var appRegistration = new AppRegistration(projectName, environment, web);
});


