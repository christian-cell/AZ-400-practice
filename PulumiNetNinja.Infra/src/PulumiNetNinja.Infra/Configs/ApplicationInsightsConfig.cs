using System;
using System.IO;
using System.Text.Json;

namespace netNinja.API.infra.Configs
{
    public class ApplicationInsightsConfig
    {
        public string AppInsightsName { get; set; } = string.Empty;
        public string WorkspaceResourceId { get; set; } = string.Empty; // Nueva propiedad para guardar el WorkspaceResourceId

        public static ApplicationInsightsConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo de configuración '{path}' no existe.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ApplicationInsightsConfigWrapper>(json)?.ApplicationInsightsConfig
                   ?? throw new InvalidOperationException("No se pudo cargar la configuración de ApplicationInsightsConfig.");
        }

        public class ApplicationInsightsConfigWrapper
        {
            public ApplicationInsightsConfig ApplicationInsightsConfig { get; set; } = new ApplicationInsightsConfig();
        }
    }
}