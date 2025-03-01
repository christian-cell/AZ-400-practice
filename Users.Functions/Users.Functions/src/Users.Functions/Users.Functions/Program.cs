using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((appBuilder, services) =>
    {
        services.Configure<OpenApiInfo>(options =>
        {
            options.Version = "v1";
            options.Title = "Azure Function API con Swagger";
            options.Description = "Descripción de la API";
            options.TermsOfService = new Uri("https://example.com/terms");
            options.Contact = new OpenApiContact
            {
                Name = "Contacto",
                Email = "contacto@example.com",
                Url = new Uri("https://example.com/contact"),
            };
            options.License = new OpenApiLicense
            {
                Name = "Licencia de Uso",
                Url = new Uri("https://example.com/license"),
            };
        });
    })
        
    .Build();

host.Run();