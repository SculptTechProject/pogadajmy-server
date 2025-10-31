using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace pogadajmy_server.Infrastructure
{
    public class AppDbFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                     ?? "Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASS};Pooling=true;Include Error Detail=true";

            var dsBuilder = new NpgsqlDataSourceBuilder(cs);
            dsBuilder.EnableDynamicJson();
            var dataSource = dsBuilder.Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(dataSource)
                .Options;

            return new AppDbContext(options);
        }
    }
}
