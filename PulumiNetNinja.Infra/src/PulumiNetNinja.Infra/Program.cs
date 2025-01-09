using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using netNinja.API.infra.Configs;
using netNinja.API.infra.Resources;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;

var storageAccountConfig = StorageAccountConfig.Load();
var webAppConfig = WebAppConfig.Load(); 
var shareConfig = ShareConfig.Load();
var sqlServerConfig = SqlServerConfig.Load();
var appInsightsConfig = ApplicationInsightsConfig.Load();

return await Pulumi.Deployment.RunAsync(() =>
{
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
        storageAccountConfig,
        accountName: $"{projectName}-stg-{environment}",
        resourceGroup: resourceGroup,
        environment: environment
    );

    // 📇 database
    var sqlServerResource = new SqlServerResource(
        sqlServerConfig: sqlServerConfig,
        shareConfig: shareConfig,
        resourceGroup: resourceGroup,
        environment: environment
    );
    
    // 📈 telemetry
    var logAnalyticsWorkspace = new Workspace($"{appInsightsConfig.AppInsightsName}-{environment}", 
        new WorkspaceArgs
        {
            ResourceGroupName = resourceGroup.ResourceGroupData.Name,
            Location = shareConfig.Location,
            Sku = new WorkspaceSkuArgs
            {
                Name = "PerGB2018"
            },
            WorkspaceName = $"{appInsightsConfig.AppInsightsName}-{environment}",
            RetentionInDays = 30
        },
        new CustomResourceOptions
        {
            DependsOn = new[] { resourceGroup.ResourceGroupData } 
        });

    UpdateAppSettingsWithWorkspaceResourceId(logAnalyticsWorkspace.Id);

    // 📈 Crear el recurso Application Insights
    var appInsights = new Component(appInsightsConfig.AppInsightsName, new ComponentArgs
    {
        ResourceName = $"{appInsightsConfig.AppInsightsName}-{environment}",
        ResourceGroupName = resourceGroup.ResourceGroupData.Name,
        Location = location,
        ApplicationType = "web",
        Kind = "web",
        WorkspaceResourceId = logAnalyticsWorkspace.Id.Apply(id => id) 
    });
    
    var sqlConnectionString = Output.Format($@"Server={sqlServerResource.SqlServerData.Name}.database.windows.net;Database={sqlServerConfig.DatabaseName}-{environment};User Id={sqlServerConfig.AdminUsername};Password={sqlServerConfig.AdminPassword};");

    var storageAccountKey = storageAccount.PrimaryStorageKey;

    var appSettings = new Dictionary<string, Output<string>>
    {
        { "ConnectionStrings__DefaultConnection", sqlConnectionString },
        { "StorageAccountKey", storageAccountKey },                      
        { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey } 
    };

    // 📄 create the web app
    var webAppResource = new WebAppResource(
        webAppConfig: webAppConfig,
        shareConfig: shareConfig,
        environment: environment,
        resourceGroup: resourceGroup,
        appSettings: appSettings
    );

    // export outputs and overwrite appsettings with results
    var outputs = new Dictionary<string, object?>
    {
        ["primaryStorageKey"] = storageAccount.PrimaryStorageKey,
        ["webAppUrl"] = Output.Format($"https://{webAppResource.WebAppData.DefaultHostName}"),
        ["sqlServerName"] = sqlServerResource.SqlServerData.Name,
        ["appInsightsInstrumentationKey"] = appInsights.InstrumentationKey,
        ["adminUsername"] = sqlServerConfig.AdminUsername
    };

    SaveFinalConfiguration(outputs);

    return outputs;
});

void SaveFinalConfiguration(Dictionary<string, object?> outputs)
{
    var filePath = "appsettings.json";
    var json = File.ReadAllText(filePath);
    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();

    config["DeploymentOutputs"] = outputs;

    File.WriteAllText(filePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
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