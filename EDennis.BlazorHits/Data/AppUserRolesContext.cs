using EDennis.BlazorHits;
using Microsoft.EntityFrameworkCore;

namespace EDennis.BlazorUtils
{
    public class DesignTimeDbContextFactory_AppUserContext : DesignTimeDbContextFactory<AppUserRolesContext>
    {
    }
    public class AppUserRolesContext : AppUserRolesContextBase
    {
        public AppUserRolesContext(DbContextOptions<AppUserRolesContext> options) : base(options)
        {
        }

        public override IEnumerable<AppRole> RoleData => TestRecords.GetAppRoles();
        public override IEnumerable<AppUser> UserData => TestRecords.GetAppUsers();
    }
}
