using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.BlazorUtils
{
    public class CrudServiceDependencies<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {

        public DbContextService<TContext> DbContextService { get; set; }
        public CountCache<TEntity> CountCache { get; set; }
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public SecurityOptions SecurityOptions { get; set; }
        public IConfiguration Configuration { get; set; }

        public CrudServiceDependencies(DbContextService<TContext> dbContextService,
            AuthenticationStateProvider authStateProvider,
            IOptionsMonitor<SecurityOptions> securityOptions,
            CountCache<TEntity> countCache,
            IConfiguration config)
        {

            DbContextService = dbContextService;
            AuthenticationStateProvider = authStateProvider;
            SecurityOptions = securityOptions.CurrentValue;
            CountCache = countCache;
            Configuration = config;
        }


        public static CrudServiceDependencies<TContext, TEntity> GetTestInstance(IConfiguration config, string userName, string role )
        {

            var securityOptions = config.GetOrThrow<SecurityOptions>("Security");
            var iomSecurityOptions = new OptionsMonitor<SecurityOptions>(securityOptions);

            var CountCache = new CountCache<TEntity>();
            var authStateProvider = new TestAuthenticationStateProvider(userName, role);
            var dbContextService = new DbContextService<TContext>(config);

            return new CrudServiceDependencies<TContext, TEntity>(
                dbContextService, authStateProvider, iomSecurityOptions, CountCache, config);

        }

        class TestAuthenticationStateProvider : AuthenticationStateProvider
        {
            public string IdpUserNameClaim { get; }
            public ClaimsPrincipal User { get; }

            public TestAuthenticationStateProvider(string userName, string role,
                string idpUserNameClaim = "preferred_username")
            {
                IdpUserNameClaim = idpUserNameClaim;
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[]
                    {
                    new Claim(IdpUserNameClaim,userName),
                    new Claim(ClaimTypes.Name,userName),
                    new Claim("name",userName),
                    new Claim("role",role),
                    new Claim(ClaimTypes.Role,role),
                    }));
            }

            public override Task<AuthenticationState> GetAuthenticationStateAsync()
                => Task.FromResult(new AuthenticationState(User));

        }


    }
}
