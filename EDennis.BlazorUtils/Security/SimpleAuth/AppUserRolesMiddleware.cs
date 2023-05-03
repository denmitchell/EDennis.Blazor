using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;


namespace EDennis.BlazorUtils.Security.SimpleAuth
{
    public class AppUserRolesMiddleware<TAppUserRolesDbContext>
        where TAppUserRolesDbContext : AppUserRolesContextBase
    {
        private readonly RequestDelegate _next;
        private readonly SecurityOptions _securityOptions;
        private readonly RolesCache _rolesCache;
        private readonly IConfiguration _config;
        public AppUserRolesMiddleware(RequestDelegate next,
            IConfiguration config,
            IOptionsMonitor<SecurityOptions> securityOptions,
            RolesCache rolesCache)
        {
            _next = next;
            _config = config;
            _securityOptions = securityOptions.CurrentValue;
            _rolesCache = rolesCache;
        }

        private static bool _eventHooked;
        private static bool _authSuccessful;
        //private static ConcurrentDictionary<string, string>

        public async Task InvokeAsync(HttpContext context, AuthenticationStateProvider authStateProvider)
        {
            if (!_eventHooked)
            {
                authStateProvider.AuthenticationStateChanged += AuthStateProvider_AuthenticationStateChanged;
                _eventHooked = true;
            }
            if (_authSuccessful)
            {
                var authState = await authStateProvider.GetAuthenticationStateAsync();

                if (!authState.User.Claims.Any(c => c.Type == "role"))
                {
                    var userName = authState.User.Claims.FirstOrDefault(c =>
                        c.Type.Equals(_securityOptions.IdpUserNameClaim,
                        StringComparison.OrdinalIgnoreCase))?.Value;

                    if (userName != null)
                    {
                        var role = GetRoleAsync(userName).Result;

                        ClaimsIdentity claimsIdentity = new();
                        claimsIdentity.AddClaims(new Claim[] {
                            new Claim("role", role),
                            new Claim(ClaimTypes.Role, role)
                        });

                        authState.User.AddIdentity(claimsIdentity);
                    }
                }
            }


            await _next(context);
        }

        private void AuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if (task.IsCompletedSuccessfully)
                _authSuccessful = true;
        }

        private async Task<string> GetRoleAsync(string userName)
        {
            using TAppUserRolesDbContext appUserDbContext = DbContextService<TAppUserRolesDbContext>.GetDbContext(_config);

            if (!_rolesCache.TryGetValue(userName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {

                var role = await (from r in appUserDbContext.AppRoles
                                  join u in appUserDbContext.AppUsers
                                      on r.Id equals u.RoleId
                                  where u.UserName == userName
                                  select r.RoleName).FirstOrDefaultAsync();

                if (role == default)
                    return "undefined"; //don't cache this

                entry = (DateTime.Now.AddMilliseconds(
                    _securityOptions.RefreshInterval), role);
                _rolesCache.AddOrUpdate(userName, entry, (u, e) => entry);
            }

            return entry.Role;
        }


    }

    public static class AppUserRolesMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppUserRoles<TAppUserRolesDbContext>(
            this IApplicationBuilder builder)
            where TAppUserRolesDbContext : AppUserRolesContextBase
        {
            return builder.UseMiddleware<AppUserRolesMiddleware<TAppUserRolesDbContext>>();
        }
    }
}
