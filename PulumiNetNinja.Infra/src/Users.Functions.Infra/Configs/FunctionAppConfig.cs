namespace Users.Functions.Infra.Configs
{
    public class FunctionAppConfig
    {
        public FunctionSku Sku { get; set; } = new FunctionSku();
        public string Runtime { get; set; } = string.Empty; // Ejemplo: "dotnet"

        public static FunctionAppConfig Load()
        {
            var config = new Pulumi.Config();
            
            return new FunctionAppConfig
            {
                Sku = new FunctionSku
                {
                    Size = config.Get("FunctionAppConfig:Sku:Size") ?? "Y1",
                    Tier = config.Get("FunctionAppConfig:Sku:Tier") ?? "Dynamic"
                },
                Runtime = config.Get("FunctionAppConfig:Runtime") ?? "dotnet"
            };
        }
    }

    public class FunctionSku
    {
        public string Size { get; set; } = string.Empty; // Ejemplo: "Y1"
        public string Tier { get; set; } = string.Empty; // Ejemplo: "Dynamic"
    }
}