using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.BlazorUtils.Security.SimpleAuth
{
    public class AppUserRolesAuthenticationStateProvider<TAppUserRolesDbContext> 
        : ServerAuthenticationStateProvider
        where TAppUserRolesDbContext : AppUserRolesContextBase
    {

        private readonly SecurityOptions _securityOptions;
        private readonly RolesCache _rolesCache;
        private readonly TAppUserRolesDbContext _appUserRolesDbContext;

        public AppUserRolesAuthenticationStateProvider(
            TAppUserRolesDbContext appUserRolesDbContext,
            IOptionsMonitor<SecurityOptions> securityOptions,
            RolesCache rolesCache) {
            _appUserRolesDbContext = appUserRolesDbContext;
            _securityOptions = securityOptions.CurrentValue;
            _rolesCache = rolesCache;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState =  await base.GetAuthenticationStateAsync();

            if (!authState.User.Claims.Any(c => c.Type == "role"))
            {
                var userName = authState.User.Claims.FirstOrDefault(c =>
                    c.Type.Equals(_securityOptions.IdpUserNameClaim,
                    StringComparison.OrdinalIgnoreCase))?.Value;

                if (userName != null)
                {
                    var role = GetRole(userName);

                    ClaimsIdentity claimsIdentity = new();
                    claimsIdentity.AddClaims(new Claim[] {
                            new Claim("role", role),
                            new Claim(ClaimTypes.Role, role)
                        });

                    authState.User.AddIdentity(claimsIdentity);
                }
            }

            return authState;
        }


        private string GetRole(string userName)
        {

            if (!_rolesCache.TryGetValue(userName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {
                    //note: this hangs if you call await ... FirstOrDefaultAsync
                    var role = (from r in _appUserRolesDbContext.AppRoles
                                join u in _appUserRolesDbContext.AppUsers
                                    on r.Id equals u.RoleId
                                where u.UserName == userName
                                select r.RoleName).FirstOrDefault();

                    if (role == default)
                        return "undefined"; //don't cache this

                    entry = (DateTime.Now.AddMilliseconds(
                        _securityOptions.RefreshInterval), role);
                    _rolesCache.AddOrUpdate(userName, entry, (u, e) => entry);


            }

            return entry.Role;
        }



    }
}
