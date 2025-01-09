using System;
using System.IO;
using System.Text.Json;

namespace netNinja.API.infra.Configs
{
    public class ShareConfig
    {
        public string ResourceGroupName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public static ShareConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo de configuración '{path}' no existe.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ShareConfigWrapper>(json)?.ShareConfig 
                   ?? throw new InvalidOperationException("No se pudo cargar la configuración.");
        }

        // Clase wrapper para mapear Json
        private class ShareConfigWrapper
        {
            public ShareConfig ShareConfig { get; set; } = new ShareConfig();
        }
    }
};

