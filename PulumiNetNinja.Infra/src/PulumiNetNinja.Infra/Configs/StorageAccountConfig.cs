using System;
using System.IO;
using System.Text.Json;

namespace netNinja.API.infra.Configs
{
    public class StorageAccountConfig
    {
        public string StorageAccountName { get; set; } = string.Empty;

        public static StorageAccountConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo de configuración '{path}' no existe.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<StorageAccountConfigWrapper>(json)?.StorageAccountConfig 
                   ?? throw new InvalidOperationException("No se pudo cargar la configuración.");
        }

        private class StorageAccountConfigWrapper
        {
            public StorageAccountConfig StorageAccountConfig { get; set; } = new StorageAccountConfig();
        }
    }
};

