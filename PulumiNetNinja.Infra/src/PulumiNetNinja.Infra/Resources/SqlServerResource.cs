using netNinja.API.infra.Configs;
using Pulumi;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace netNinja.API.infra.Resources
{
    public class SqlServerResource
    {
        public Server SqlServerData { get; }
        public Database SqlDatabaseData { get; }

        public SqlServerResource(SqlServerConfig sqlServerConfig, ShareConfig shareConfig, ResourceGroup resourceGroup, string environment)
        {
            
            // Aquí utilicé sqlServerConfig.ServerName para crear el recurso Server.
            SqlServerData = new Server($"{sqlServerConfig.ServerName}-{environment}", new ServerArgs
            {
                Location = shareConfig.Location,
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                AdministratorLogin = sqlServerConfig.AdminUsername,
                AdministratorLoginPassword = sqlServerConfig.AdminPassword,
                Version = "12.0", 
                ServerName = $"{sqlServerConfig.ServerName}-{environment}"
            });

            SqlDatabaseData = new Database($"{sqlServerConfig.DatabaseName}-{environment}", new DatabaseArgs
                {
                    ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                    ServerName = SqlServerData.Name.Apply(n => n),
                    CreateMode = "Default",
                    Location = shareConfig.Location,
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
}