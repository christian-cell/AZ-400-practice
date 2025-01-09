using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using User.Domain.Infraestructure;
using User.Services.Configurations;
using User.Services.MapperProfiles;
using Swashbuckle.AspNetCore.SwaggerGen;


/**/

namespace Users.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json");

            builder.Services.AddServices();
            
            builder.Services
                .AddAutoMapper(typeof(UserMapperProfile).Assembly, typeof(User.Domain.Entities.User).Assembly);
            
            /*
             * just a reminder : stop program compilation and go to Domains location
             * dotnet ef migrations add Users --context UserDbContext --output-dir Migrations --startup-project ../Users.API
             * dotnet ef database update --context UserDbContext --startup-project ../Users.API
             */
            builder.Services.AddDbContextDependencies(builder.Configuration["Azure:SQL:ConnectionString"]!);

            builder.Services.AddControllers();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoodDeal API", Version = "v1" });
                
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
                
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Donations API V1");
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
            
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                }
            }

            app.Run();
        }
    }
};