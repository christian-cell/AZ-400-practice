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

        public SqlServerResource(SqlServerConfig sqlServerConfig, ResourceGroup resourceGroup, SqlServerConfiguration sqlServerConfiguration )
        {
            
            SqlServerData = new Server(sqlServerConfiguration.ServerName, new ServerArgs
            {
                Location = sqlServerConfiguration.Location,
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                AdministratorLogin = sqlServerConfig.AdminUsername,
                AdministratorLoginPassword = sqlServerConfig.AdminPassword,
                Version = "12.0", 
                ServerName = sqlServerConfiguration.ServerName
            });

            SqlDatabaseData = new Database($"{sqlServerConfiguration.ProjectName}-{sqlServerConfiguration.Environment}", new DatabaseArgs
                {
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    ServerName = sqlServerConfiguration.ServerName ,
                    CreateMode = "Default",
                    Location = sqlServerConfiguration.Location,
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

