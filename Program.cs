using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Npgsql;
using pogadajmy_server.Infrastructure;
using System.Text;
using System.Threading.RateLimiting;

namespace pogadajmy_server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // --- ENV / config ---
        var jwtKey = Environment.GetEnvironmentVariable("POGADAJMY_JWT_KEY");
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("POGADAJMY_JWT_KEY is not set.");
        }
        var jwtIssuer   = Environment.GetEnvironmentVariable("POGADAJMY_JWT_ISSUER") ?? "pogadajmy";
        var jwtAudience = Environment.GetEnvironmentVariable("POGADAJMY_JWT_AUDIENCE") ?? "pogadajmy-api";


        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen();
        
        // AuthN/AuthZ
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false; // PROD: true za SSL/ingress
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,   ValidIssuer   = jwtIssuer,
                    ValidateAudience = true, ValidAudience = jwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        // Database context
        var csFromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        var cs = !string.IsNullOrWhiteSpace(csFromEnv)
            ? csFromEnv
            : builder.Configuration.GetConnectionString("Default"); // appsettings, itp.

        try
        {
            var csSafe = new NpgsqlConnectionStringBuilder(cs) { Password = "****" };
            Console.WriteLine($"[DB] Effective CS -> Host={csSafe.Host}; Port={csSafe.Port}; Database={csSafe.Database}; Username={csSafe.Username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB] Could not parse CS: {ex.Message}");
        }

        // Rate limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("ingest", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var sourceKey = httpContext.Request.RouteValues.TryGetValue("sourceKey", out var v) ? v?.ToString() : "none";
                var partitionKey = $"{ip}:{sourceKey}";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,                 // 60 żądań
                        Window = TimeSpan.FromMinutes(1), // w 1 minutę
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0                    // bez kolejki
                    });
            });
            // 429 zamiast domyślnego
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        
        // Max body size
        builder.WebHost.ConfigureKestrel(o =>
        {
            o.Limits.MaxRequestBodySize = 256 * 1024; // 256 KB
        });

        // Npgsql + dynamic JSON
        var dsBuilder = new NpgsqlDataSourceBuilder(cs);
        dsBuilder.EnableDynamicJson();
        var dataSource = dsBuilder.Build();

        // DB context
        builder.Services.AddSingleton(dataSource);
        builder.Services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            var ds = sp.GetRequiredService<NpgsqlDataSource>();
            opt.UseNpgsql(ds);
        });
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "pogadajmy-server",
                Version = "v1"
            });
        });

        var app = builder.Build();
        
        // Auto-migrate db
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "pogadajmy-server v1");
                c.RoutePrefix = "swagger";
            });
        }

        // app.UseHttpsRedirection(); // PROD: true za SSL/ingress 

        app.UseAuthentication(); 
        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/health");
        app.MapGet("/dbhealth", (AppDbContext db) => db.Database.CanConnectAsync());

        app.Run();
    }
}
