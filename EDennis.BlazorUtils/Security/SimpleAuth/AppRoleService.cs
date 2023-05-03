namespace EDennis.BlazorUtils
{
    public class AppRoleService<TAppUserRolesDbContext> : CrudService<TAppUserRolesDbContext, AppRole>
        where TAppUserRolesDbContext : AppUserRolesContextBase
    {
        public AppRoleService(CrudServiceDependencies<TAppUserRolesDbContext, AppRole> deps) : base(deps) { }

        public override void BeforeDelete(AppRole existing)
        {
            DbContext.Set<AppUser>()
                .Where(u => u.RoleId == existing.Id)
                .ToList()
                .ForEach(u => u.RoleId = null);
            DbContext.SaveChanges();            
        }

    }
}
