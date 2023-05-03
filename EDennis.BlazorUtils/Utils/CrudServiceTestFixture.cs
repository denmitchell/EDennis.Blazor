using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace EDennis.BlazorUtils
{
    public class CrudServiceTestFixture<TContext, TService, TEntity>
        where TContext : DbContext
        where TEntity : class
        where TService : CrudService<TContext, TEntity>
    {
        private readonly static ConcurrentDictionary<string, IConfiguration> _configs = new();


        public TService GetCrudService(string appsettingsFile, string userName, string role,
            DbContextType dbContextType = DbContextType.SqlServerOpenTransaction,
            ITestOutputHelper output = null)
        {
            var appsettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), appsettingsFile);
            if (!File.Exists(appsettingsFilePath))
                throw new Exception($"Invalid appsettings path: {appsettingsFilePath}");

            if (!_configs.TryGetValue(appsettingsFile, out var config)) {
                config = new ConfigurationBuilder()
                    .AddJsonFile(appsettingsFilePath)
                    .AddEnvironmentVariables()
                    .Build();
                _configs.TryAdd(appsettingsFile, config);
            }

            var deps = CrudServiceDependencies<TContext, TEntity>.GetTestInstance(config, userName, role);

            TService service = (TService)Activator.CreateInstance(typeof(TService),deps);

            if(dbContextType != DbContextType.SqlServer)
            {
                service.SetDbContext(dbContextType, output);
            }

            return service;
        }

    }
}
