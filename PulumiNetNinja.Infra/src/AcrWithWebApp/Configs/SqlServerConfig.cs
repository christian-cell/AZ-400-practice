using System;
using System.IO;
using System.Text.Json;

namespace AcrWithWebApp.Configs
{
    public class SqlServerConfig
    {
        public string ServerName { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public static SqlServerConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo de configuración '{path}' no existe.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SqlServerConfigWrapper>(json)?.SqlServerConfig
                   ?? throw new InvalidOperationException("No se pudo cargar la configuración de SqlServerConfig.");
        }

        private class SqlServerConfigWrapper
        {
            public SqlServerConfig SqlServerConfig { get; set; } = new SqlServerConfig();
        }
    }
};

