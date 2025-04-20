using System;
using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace Users.Functions.Infra.Resources
{
    public class StorageAccountResource
    {
        public StorageAccount StorageAccount { get; }
        public Output<string> PrimaryStorageKey { get; }

        public Output<string> ConnectionString { get; }

        public StorageAccountResource(string accountName, ResourceGroup resourceGroup, string environment)
        {
            Console.WriteLine($"Creating Storage Account {accountName}");
            Console.WriteLine($"resource group name {resourceGroup.ResourceGroupData.Name}-{environment}");

            StorageAccount = new StorageAccount(accountName, new StorageAccountArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS
                },
                Kind = Kind.StorageV2,
                AccountName = accountName
            });

            var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                AccountName = StorageAccount.Name
            });

            PrimaryStorageKey = storageAccountKeys.Apply(accountKeys =>
            {
                var firstKey = accountKeys.Keys[0].Value;
                return Output.CreateSecret(firstKey);
            });

            // 🧩 Construir connection string
            ConnectionString = Output.Tuple(StorageAccount.Name, PrimaryStorageKey).Apply(items =>
            {
                var name = items.Item1;
                var key = items.Item2;
                var connStr = $"DefaultEndpointsProtocol=https;AccountName={name};AccountKey={key};EndpointSuffix=core.windows.net";
                return Output.CreateSecret(connStr);
            });
        }
    }
};