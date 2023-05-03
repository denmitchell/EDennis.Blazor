using SqlServerExceptions = EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions;
using SqliteExceptions = EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using Xunit.Abstractions;

namespace EDennis.BlazorUtils
{
    /// <summary>
    /// Singleton
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class DbContextService<TContext> 
        where TContext : DbContext
    {

        #region Variables

        private readonly string _sqlServerConnectionString;
        
        public string ConfigurationSectionKey { get; set; } = "DbContexts";

        #endregion
        #region Constructor

        public DbContextService(IConfiguration config = null)
        {
            _sqlServerConnectionString = GetConnectionString(config, ConfigurationSectionKey);
        }

        #endregion
        #region Testing

        public TContext GetTestDbContext(DbContextType dbContextType, ITestOutputHelper output = null)
        {
            if (dbContextType == DbContextType.SqlServer)
            {
                var builder = new DbContextOptionsBuilder<TContext>();
                builder.UseSqlServer(_sqlServerConnectionString)
                    .EnableSensitiveDataLogging();

                if (output != null)
                    builder.LogTo(output.WriteLine);

                SqlServerExceptions.UseExceptionProcessor(builder);

                TContext context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                return context;

            }
            else if (dbContextType == DbContextType.SqlServerOpenTransaction)
            {
                DbConnection connection = new SqlConnection(_sqlServerConnectionString);

                var builder = new DbContextOptionsBuilder<TContext>();
                builder.UseSqlServer(connection)
                    .EnableSensitiveDataLogging();

                if (output != null)
                    builder.LogTo(output.WriteLine);

                SqlServerExceptions.UseExceptionProcessor(builder);

                connection.Open();
                var transaction = connection.BeginTransaction();

                TContext context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                context.Database.UseTransaction(transaction);
                return context;

            } else
            {
                var connection = new SqliteConnection("Data Source=:memory:");

                var builder = new DbContextOptionsBuilder<TContext>();
                builder.UseSqlite(connection)
                    .EnableSensitiveDataLogging();

                if (output != null)
                    builder.LogTo(output.WriteLine);

                SqliteExceptions.UseExceptionProcessor(builder);

                connection.Open();

                TContext context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                context.Database.EnsureCreated();
                return context;
            }
        }

        #endregion
        #region Production


        public static TContext GetDbContext(IConfiguration config,
            string sectionKey = "DbContexts")
            => GetDbContext(GetConnectionString(config, sectionKey));


        public static DbContextOptions<TContext> GetDbContextOptions(IConfiguration config,
            string sectionKey = "DbContexts")
            => GetDbContextOptions(GetConnectionString(config, sectionKey));


        public static DbContextOptions<TContext> GetDbContextOptions(string cxnString)
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlServer(cxnString);
            SqlServerExceptions.UseExceptionProcessor(builder);

            return builder.Options;
        }

        public static string GetConnectionString(IConfiguration config, string sectionKey = "DbContexts")
        {
            var cxnString = config.GetSection($"{sectionKey}:{typeof(TContext).Name}").Get<string>();
            if (string.IsNullOrEmpty(cxnString))
                throw new ApplicationException($"Connection string for {typeof(TContext).Name} " +
                    $"not defined in Configuration (e.g., appsettings)");

            return cxnString;
        }


        public static TContext GetDbContext(string cxnString)
        {
            var options = GetDbContextOptions(cxnString);
            TContext context = (TContext)Activator.CreateInstance(typeof(TContext), options);
            return context;
        }

        public TContext GetDbContext(IConfiguration config)
            => GetDbContext(GetConnectionString(config));

        #endregion
    }
}
