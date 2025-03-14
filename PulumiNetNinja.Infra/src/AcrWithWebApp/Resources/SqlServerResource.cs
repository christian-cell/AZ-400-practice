using AcrWithWebApp.Configs;
using Pulumi;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace AcrWithWebApp.Resources
{
    public class SqlServerResource
    {
        public Server SqlServerData { get; }
        public Database SqlDatabaseData { get; }

        public SqlServerResource(SqlServerConfig sqlServerConfig, ResourceGroup resourceGroup, string environment, string serverName, string projectName, string location)
        {
            
            SqlServerData = new Server(serverName, new ServerArgs
            {
                Location = location,
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                AdministratorLogin = sqlServerConfig.AdminUsername,
                AdministratorLoginPassword = sqlServerConfig.AdminPassword,
                Version = "12.0", 
                ServerName = serverName
            });

            SqlDatabaseData = new Database($"{projectName}-{environment}", new DatabaseArgs
                {
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    ServerName = serverName ,
                    CreateMode = "Default",
                    Location = location,
                    Sku = new SkuArgs
                    {
                        Name = "S0",
                        Tier = "Standard",
                        Capacity = 10
                    }
                },
                new CustomResourceOptions
                {
                    DependsOn = {SqlServerData}
                });
        }
    }
};

