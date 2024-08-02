using Predictrix.Application;
using Predictrix.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

namespace Predictrix
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var connectionString = builder.Configuration.GetConnectionString("Predictrix")
                                   ?? throw new InvalidOperationException("Connection string not found!");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Discord(configuration.GetValue<ulong>("Discord:WebHookId"), LogEventLevel.Information, configuration.GetValue<string>("Discord:WebHookToken") ?? "")
                .CreateLogger();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(connectionString);
            builder.Services.AddApplicationServices();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
