using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pulumi;
using Users.Functions.Infra.Configs;
using Users.Functions.Infra.Resources;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;

return await Pulumi.Deployment.RunAsync(() =>
{
    var shareConfig = ShareConfig.Load();
    var sqlServerConfig = SqlServerConfig.Load();
    var functionAppConfig = FunctionAppConfig.Load();
    
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
        resourceGroup: resourceGroup,
        environment: environment
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
    
    var logAnalyticsWorkspace = new Workspace($"{projectName}-insights-{environment}", 
    new WorkspaceArgs
    {
        ResourceGroupName = resourceGroup.ResourceGroupData.Name.Apply(name => name),
        Location = shareConfig.Location,
        Sku = new WorkspaceSkuArgs
        {
            Name = "PerGB2018" 
        },
        WorkspaceName = $"{projectName}-insights-{environment}",
        RetentionInDays = 30 
    },
    new CustomResourceOptions
    {
        DependsOn = new[] { resourceGroup.ResourceGroupData } // Prioriza la dependencia en el grupo de recursos
    });

    // Actualizar la configuración con el ID del Workspace
    UpdateAppSettingsWithWorkspaceResourceId(logAnalyticsWorkspace.Id);

    var appInsights = new Component($"{projectName}-insights-{environment}", new ComponentArgs
    {
        ResourceName = $"{projectName}-insights-{environment}",
        ResourceGroupName = resourceGroup.ResourceGroupData.Name.Apply(name => name), // Corrige uso de Output<string>
        Location = location,
        ApplicationType = "FunctionApp",
        Kind = "functionapp",
        WorkspaceResourceId = logAnalyticsWorkspace.Id.Apply(id => id) // Asegura el uso correcto del Output<string>
    }, new CustomResourceOptions
    {
        DependsOn = new[] { logAnalyticsWorkspace } // Asegúrate de que espere al workspace
    });
    
    var serverName = sqlServerResource.SqlServerData.Name.Apply(name => name);
    var sqlConnectionString = Output.Format($@"Server={serverName}.database.windows.net;Database={sqlServerConfig.DatabaseName}-{environment};User Id={sqlServerConfig.AdminUsername};Password={sqlServerConfig.AdminPassword};");
    
    var storageAccountKey = storageAccount.PrimaryStorageKey;
    
    // 🚀 Crear la Function App
    var functionAppResource = new FunctionAppResource(
        functionName: $"{projectName}-functionapp-{environment}",
        functionAppConfig: functionAppConfig,
        shareConfig: shareConfig,
        environment: environment,
        resourceGroup: resourceGroup,
        appSettings: new Dictionary<string, Output<string>>
        {
            { "AzureWebJobsStorage", storageAccountKey }, 
            { "FUNCTIONS_WORKER_RUNTIME", Output.Create(functionAppConfig.Runtime) },
            { "FUNCTIONS_EXTENSION_VERSION", Output.Create("~4") },
            { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey }, 
            { "SQL_CONNECTION_STRING", sqlConnectionString } 
        }
        /* TODO */
        /* depend on stg to pick the storageAccountKey after stg creation */
    );

    var outputs = new Dictionary<string, object?>
    {
        ["primaryStorageKey"] = storageAccount.PrimaryStorageKey,
        ["functionAppUrl"] = Output.Format($"https://{functionAppResource.FunctionApp.DefaultHostName}"),
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