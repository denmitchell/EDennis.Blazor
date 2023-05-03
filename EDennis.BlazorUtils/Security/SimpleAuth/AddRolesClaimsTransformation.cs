using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.BlazorUtils.Security.SimpleAuth
{
    public class UserRolesClaimsTransformation<TAppUserRolesDbContext> : IClaimsTransformation
        where TAppUserRolesDbContext : AppUserRolesContextBase
    {
        private readonly SecurityOptions _securityOptions;
        private readonly RolesCache _rolesCache;
        private readonly TAppUserRolesDbContext _appUserDbContext;

        public UserRolesClaimsTransformation(IOptionsMonitor<SecurityOptions> securityOptions,
            TAppUserRolesDbContext appUserDbContext,
            RolesCache rolesCache) {
            _securityOptions = securityOptions.CurrentValue;
            _rolesCache = rolesCache;
            _appUserDbContext = appUserDbContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Claims.Any(c => c.Type == "role"))
                return principal;

            var userName = principal.Claims.FirstOrDefault(c =>
                c.Type.Equals(_securityOptions.IdpUserNameClaim,
                StringComparison.OrdinalIgnoreCase))?.Value;

            if (userName == null)
                return principal;

            var role = await GetRoleAsync(userName);


            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaims( new Claim[] {
                new Claim("role", role),
                new Claim(ClaimTypes.Role, role)
            });

            principal.AddIdentity(claimsIdentity);
            return principal;
        }

        private async Task<string> GetRoleAsync(string userName)
        {

            if (!_rolesCache.TryGetValue(userName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {

                var role = await(from r in _appUserDbContext.AppRoles
                                 join u in _appUserDbContext.AppUsers
                                     on r.Id equals u.RoleId
                                 where u.UserName == userName
                                 select r.RoleName).FirstOrDefaultAsync();

                if(role == default)
                    return "undefined"; //don't cache this

                entry = (DateTime.Now.AddMilliseconds(
                    _securityOptions.RefreshInterval), role);
                _rolesCache.AddOrUpdate(userName, entry, (u, e) => entry);
            }

            return entry.Role;
        }
    }
}
