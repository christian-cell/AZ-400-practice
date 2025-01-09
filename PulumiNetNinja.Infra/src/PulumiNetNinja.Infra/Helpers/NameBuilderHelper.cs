using System;
using System.Text.RegularExpressions;

namespace netNinja.API.infra.Helpers
{
    public class NameBuilderHelper
    {
        public static string NormalizeResourceGroupName(string baseName, string environment)
        {
            // Combinar nombre base y entorno
            var combinedName = $"{baseName}-{environment}";

            // Recortar a 90 caracteres si es demasiado largo
            if (combinedName.Length > 90)
            {
                combinedName = combinedName.Substring(0, 90);
            }

            // Validar formato permitido
            if (!Regex.IsMatch(combinedName, @"^[-\w\._\(\)]+$"))
            {
                throw new InvalidOperationException($"El nombre generado '{combinedName}' contiene caracteres no válidos.");
            }

            return combinedName;
        }
    }
};

