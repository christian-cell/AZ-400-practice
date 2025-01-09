using System;
using System.IO;
using System.Text.Json;

namespace netNinja.API.infra.Configs
{
    public class WebAppConfig
    {
        public string AppName { get; set; } = string.Empty;
        public string AppServicePlanName { get; set; } = string.Empty;
        public SkuConfig Sku { get; set; } = new SkuConfig();

        public static WebAppConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo de configuración '{path}' no existe.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<WebAppConfigWrapper>(json)?.WebAppConfig 
                   ?? throw new InvalidOperationException("No se pudo cargar la configuración para WebAppConfig.");
        }

        public class SkuConfig
        {
            public string Size { get; set; } = string.Empty;
            public string Tier { get; set; } = string.Empty;
        }

        private class WebAppConfigWrapper
        {
            public WebAppConfig WebAppConfig { get; set; } = new WebAppConfig();
        }
    }
};
