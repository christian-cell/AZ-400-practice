using Microsoft.OpenApi.Models;

namespace UsersACA.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json");

            /*
             * just a reminder : stop program compilation and go to Domains location
             * dotnet ef migrations add Users --context UserDbContext --output-dir Migrations --startup-project ../Users.API
             * dotnet ef database update --context UserDbContext --startup-project ../Users.API
             */

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users-ACA API", Version = "v1" });
            });

            builder.Services.AddCors((options) =>
            {
                options.AddPolicy("DevCors", (corsBuilder) =>
                {
                    corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:5221")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });


            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Users-ACA API V1");
                c.OAuthScopeSeparator(" ");
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseCors("DevCors");
            }

            app.UseHttpsRedirection();

            /*app.UseAuthentication();

            app.UseAuthorization();*/

            app.MapControllers();

            app.Run();
        }
    }
}
    
    
    

