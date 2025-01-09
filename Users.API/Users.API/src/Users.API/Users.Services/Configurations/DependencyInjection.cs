using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User.Domain.Infraestructure;
using User.Services.Abstractions;
using User.Services.Services;

namespace User.Services.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            
            return services;
        }
        
        public static IServiceCollection AddDbContextDependencies(this IServiceCollection services, string cnx)
        {
            services.AddDbContext<DbContext, UserDbContext>(options =>
            {
                var mycnx = cnx;
                options.UseSqlServer(cnx);
            });

            return services;
        }
    }
};

