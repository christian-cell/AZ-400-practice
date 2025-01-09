using System;
using netNinja.API.infra.Configs;
using netNinja.API.infra.Helpers;
using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace netNinja.API.infra.Resources
{
    public class StorageAccountResource
    {
        public StorageAccount StorageAccount { get; }
        public Output<string> PrimaryStorageKey { get; }

        public StorageAccountResource(/*StorageAccountConfig config*/ string accountName, ResourceGroup resourceGroup, string environment)
        {
            Console.WriteLine($"Creating Storage Account {accountName}-{environment}");
            Console.WriteLine($"resource group name {resourceGroup.ResourceGroupData.Name}-{environment}");
            // Crear el recurso del Storage Account

            //var rgName = NameBuilderHelper.NormalizeResourceGroupName(resourceGroup.ResourceGroupData.Name, environment);
            
            StorageAccount = new StorageAccount(accountName, new StorageAccountArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS
                },
                Kind = Kind.StorageV2,
                AccountName = accountName/*$"{config.StorageAccountName}{environment}"*/ //$"{config.StorageAccountName}-{environment}" name not allowed
            });

            // Obtener las claves del Storage Account
            var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
            {
                ResourceGroupName = resourceGroup.ResourceGroupData.Name,
                AccountName = StorageAccount.Name
            });

            // Obtener y exportar la clave primaria como Output
            PrimaryStorageKey = storageAccountKeys.Apply(accountKeys =>
            {
                var firstKey = accountKeys.Keys[0].Value;
                return Output.CreateSecret(firstKey);
            });
        }
    }
};

