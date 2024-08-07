using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Predictrix.Application;
using Predictrix.Infrastructure;
using Predictrix.Middlewares;
using Predictrix.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

namespace Predictrix
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var connectionString = builder.Configuration.GetConnectionString("Predictrix")
                                   ?? throw new InvalidOperationException("Connection string not found!");
            var jwtSecret = configuration["Authentication:Jwt:SecretKey"]
                           ?? throw new InvalidOperationException("JWT secret key not found!");
            
            // builder.Configuration.AddJsonFile("secrets.json", true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Discord(configuration.GetValue<ulong>("Discord:WebHookId"), LogEventLevel.Information, configuration.GetValue<string>("Discord:WebHookToken") ?? "")
                .CreateLogger();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(connectionString);
            builder.Services.AddApplicationServices();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDictionarySerializerService, DictionarySerializerService>();
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Authentication:Jwt:Issuer"],
                        ValidAudience = configuration["Authentication:Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        NameClaimType = JwtRegisteredClaimNames.Sub,
                    };
                });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<ExceptionMiddleware>();
            
            app.WarmUp();

            app.Run();
        }

        private static void WarmUp(this IApplicationBuilder app)
        {
            Log.Information("Warming up the application...");
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            _ = dbContext?.Users.OrderBy(u => u.UserId).FirstOrDefault();
            Log.Information("Application warmed up");
        }
    }
}
