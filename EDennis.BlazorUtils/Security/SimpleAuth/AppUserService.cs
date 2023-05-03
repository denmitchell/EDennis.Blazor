namespace EDennis.BlazorUtils
{
    public class AppUserService<TAppUserRolesDbContext> : CrudService<TAppUserRolesDbContext, AppUser>
        where TAppUserRolesDbContext : AppUserRolesContextBase
    {
        public AppUserService(CrudServiceDependencies<TAppUserRolesDbContext, AppUser> deps) : base(deps) { } 
    }
}
