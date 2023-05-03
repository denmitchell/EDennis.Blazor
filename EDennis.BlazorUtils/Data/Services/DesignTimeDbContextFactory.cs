using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EDennis.BlazorUtils
{
    public abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json")
                .Build();

            var cxnString = DbContextService<TContext>.GetConnectionString(config);

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlServer(cxnString)
                .EnableSensitiveDataLogging()
                .UseExceptionProcessor();

            return (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
        }

    }
}
