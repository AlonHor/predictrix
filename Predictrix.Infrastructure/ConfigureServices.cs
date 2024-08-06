using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Predictrix.Infrastructure
{
    public static class ConfigureServices
    {
        public static void AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString), b =>
                {
                    b.MigrationsAssembly("Predictrix");
                    b.EnableRetryOnFailure();
                }));

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }
    }
}
